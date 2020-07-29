using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using PrDash.Configuration;
using PrDash.View;

namespace PrDash.DataSource
{
    /// <summary>
    /// An implementation of <see cref="IPullRequestSource"/> which retrieves the active
    /// pull requests for a user, from the Azure DevOps server.
    /// </summary>
    public class AzureDevOpsPullRequestSource : IPullRequestSource
    {
        /// <summary>
        /// The configuration that we should use to connect to the backing store.
        /// </summary>
        private readonly Config m_config;

        /// <summary>
        /// The running statistics of pull requests that we are tracking.
        /// </summary>
        private PullRequestStatistics m_statistics = new PullRequestStatistics();

        /// <summary>
        /// Constructs a new request source.
        /// </summary>
        /// <param name="config">The configuration to driver the system.</param>
        public AzureDevOpsPullRequestSource(Config config)
        {
            m_config = config;
        }

        /// <summary>
        /// Event handler for receiving updates to the pull request statistics.
        /// </summary>
        public event EventHandler<StatisticsUpdateEventArgs>? StatisticsUpdate;

        /// <summary>
        /// Retrieves pull requests from the configured data source.
        /// </summary>
        /// <returns>An async stream of <see cref="PullRequestViewElement"/></returns>
        public IAsyncEnumerable<PullRequestViewElement> FetchAssignedPullRequests(PrState state)
        {
            m_statistics.Reset();

            return FetchPullRequstsInternal(state);
        }

        /// <summary>
        /// Retrieves all active pull requests this user has created.
        /// </summary>
        /// <returns>An async stream of <see cref="PullRequestViewElement"/></returns>
        public async IAsyncEnumerable<PullRequestViewElement> FetchCreatedPullRequests()
        {
            foreach (var accountGroup in m_config.AccountsByUri)
            {
                Uri organizationUri = accountGroup.Key;

                using VssConnection connection = await GetConnectionAsync(organizationUri, accountGroup.Value);
                using GitHttpClient client = await connection.GetClientAsync<GitHttpClient>();
                {
                    // Capture the currentUserId so it can be used to filter PR's later.
                    //
                    Guid userId = connection.AuthorizedIdentity.Id;

                    // Only fetch pull requests which are active, and assigned to this user.
                    //
                    GitPullRequestSearchCriteria criteria = new GitPullRequestSearchCriteria
                    {
                        CreatorId = userId,
                        Status = PullRequestStatus.Active,
                        IncludeLinks = false,
                    };

                    foreach (AccountConfig account in accountGroup.Value)
                    {
                        List<GitPullRequest> requests = await client.GetPullRequestsByProjectAsync(account.Project, criteria);
                        foreach (var request in requests)
                        {
                            yield return new PullRequestViewElement(request, account.Handler!) { CreatedMode = true };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Helper function to make async code line up, since interface methods cannot be marked
        /// as async.
        /// </summary>
        /// <returns>An async stream of <see cref="PullRequestViewElement"/></returns>
        private async IAsyncEnumerable<PullRequestViewElement> FetchPullRequstsInternal(PrState state)
        {
            foreach (var accountGroup in m_config.AccountsByUri)
            {
                Uri organizationUri = accountGroup.Key;

                // Create a shared connection to the AzureDevOps Git API for all accounts sharing the same organization uri.
                //
                using VssConnection connection = await GetConnectionAsync(organizationUri, accountGroup.Value);
                using GitHttpClient client = await connection.GetClientAsync<GitHttpClient>();

                // Capture the currentUserId so it can be used to filter PR's later.
                //
                Guid userId = connection.AuthorizedIdentity.Id;

                foreach (AccountConfig account in accountGroup.Value)
                {
                    await foreach (var pr in FetchPullRequests(client, userId, account, state))
                    {
                        // We only want to fetch the commit data if the config is enabled.
                        //
                        if (m_config.SortByRecentCommit)
                        {
                            var commits = await client.GetPullRequestCommitsAsync(pr.Repository.Id, pr.PullRequestId);
                            pr.Commits = commits.ToArray();
                        }

                        yield return new PullRequestViewElement(pr, account.Handler!);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all active & actionable pull requests to the configured data source.
        /// </summary>
        /// <param name="accountConfig">The account to retrieve the pull requests for.</param>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        private async IAsyncEnumerable<GitPullRequest> FetchPullRequests(GitHttpClient client, Guid userId, AccountConfig accountConfig, PrState state)
        {
            // Gets the "processed" state of a PR.
            //
            async Task<PrState?> getState(GitPullRequest pr)
            {
                // If the PR is in draft, it's a draft.
                //
                if (pr.IsDraft == true)
                {
                    return PrState.Drafts;
                }

                // Try to find our selves in the reviewer list.
                //
                if (!TryGetReviewer(pr, userId, out IdentityRefWithVote reviewer))
                {
                    //  Skip this review if we aren't assigned.
                    //
                    return null;
                }

                // Skip declined reviews.
                //
                if (reviewer.HasDeclined.HasValue && reviewer.HasDeclined.Value)
                {
                    return null;
                }

                // If we have already casted a "final" vote, then skip it.
                //
                if (reviewer.HasFinalVoteBeenCast())
                {
                    return PrState.SignedOff;
                }

                if (reviewer.IsWaiting())
                {
                    // If we are waiting on the PR, inspect the active threads in the PR.
                    // If we have left a comment in a thread that is still active, the PR is not actionable to us.
                    // If we there are no active threads where we have participated, the PR is actionable to us.
                    //
                    List<GitPullRequestCommentThread> threads = await client.GetThreadsAsync(pr.Repository.Id, pr.PullRequestId);

                    bool threadInvolvesUs(GitPullRequestCommentThread thread) => thread.Comments.Any(c => Guid.Parse(c.Author.Id) == userId);
                    bool anyActiveThreads = threads.Any(t => t.Status == CommentThreadStatus.Active && threadInvolvesUs(t));
                    return anyActiveThreads ? PrState.Waiting : PrState.Actionable;
                }
                else
                {
                    // If these criteria haven't been met, then the PR is actionable.
                    //
                    return PrState.Actionable;
                }
            }

            await foreach (var pr in FetchAccountActivePullRequsts(client, userId, accountConfig))
            {
                PrState? processedState = await getState(pr);
                
                m_statistics.Accumulate(processedState);
                if (state == processedState)
                {
                    yield return pr;
                }
            }

            // Post event on stats update.
            //
            OnStatisticsUpdate();
        }

        /// <summary>
        /// Retrieves all active & actionable pull requests for a specific account.
        /// </summary>
        /// <param name="accountConfig">The account to get the pull requests for.</param>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        private static async IAsyncEnumerable<GitPullRequest> FetchAccountActivePullRequsts(GitHttpClient client, Guid userId, AccountConfig accountConfig)
        {
            // Only fetch pull requests which are active, and assigned to this user.
            //
            GitPullRequestSearchCriteria criteria = new GitPullRequestSearchCriteria
            {
                ReviewerId = userId,
                Status = PullRequestStatus.Active,
                IncludeLinks = false,
            };

            List<GitPullRequest> requests = await client.GetPullRequestsByProjectAsync(accountConfig.Project, criteria);
            foreach (var request in requests)
            {
                yield return request;
            }
        }

        /// <summary>
        /// Tries to get the current users reviewer object from a pull request.
        /// </summary>
        /// <param name="pullRequest">The pull request we want to look our selves up in.</param>
        /// <param name="currentUserId">The <see cref="Guid"/> of our current user.</param>
        /// <param name="reviewer">Output  parameter that points to our own reviewer object.</param>
        /// <returns>Returns <c>true</c> if the reviewer was found, <c>false</c> otherwise.</returns>
        private static bool TryGetReviewer(GitPullRequest pullRequest, Guid currentUserId, out IdentityRefWithVote reviewer)
        {
            foreach (IdentityRefWithVote r in pullRequest.Reviewers)
            {
                if (currentUserId.Equals(Guid.Parse(r.Id)))
                {
                    reviewer = r;
                    return true;
                }
            }

            reviewer = new IdentityRefWithVote();
            return false;
        }

        /// <summary>
        /// Factory method for creating the connection.
        /// </summary>
        /// <param name="account">Account details to create the connection for.</param>
        /// <returns>A valid <see cref="VssConnection"/> for the given account.</returns>
        private static async Task<VssConnection> GetConnectionAsync(Uri organizationUri, IList<AccountConfig> accounts)
        {
            VssCredentials credential;

            // If the org has more than one pat token, just pick one, it doesn't matter.
            //
            AccountConfig? patTokenAccount = accounts.FirstOrDefault(a => a.PersonalAccessToken != null);

            // If the user didn't configure a PAT token, try to login via AAD.
            //
            if (patTokenAccount == null)
            {
                credential = new VssAadCredential(UserPrincipal.Current.EmailAddress);
            }
            else
            {
                credential = new VssBasicCredential(string.Empty, patTokenAccount.PersonalAccessToken);
            }

            VssConnection connection = new VssConnection(organizationUri, credential);
            await connection.ConnectAsync();
            return connection;
        }

        /// <summary>
        /// Invokes event update when statistics are updated.
        /// </summary>
        private void OnStatisticsUpdate()
        {
            StatisticsUpdateEventArgs eventArgs = new StatisticsUpdateEventArgs()
            {
                Statistics = m_statistics
            };

            StatisticsUpdate?.Invoke(this, eventArgs);
        }
    }
}
