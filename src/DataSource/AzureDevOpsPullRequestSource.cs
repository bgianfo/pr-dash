using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
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
        public IAsyncEnumerable<PullRequestViewElement> FetchPullRequests(PrState state)
        {
            m_statistics.Reset();

            return FetchPullRequstsInternal(state);
        }

        /// <summary>
        /// Helper function to make async code line up, since interface methods cannot be marked
        /// as async.
        /// </summary>
        /// <returns>An async stream of <see cref="PullRequestViewElement"/></returns>
        private async IAsyncEnumerable<PullRequestViewElement> FetchPullRequstsInternal(PrState state)
        {
            foreach (AccountConfig account in m_config.Accounts)
            {
                using (VssConnection connection = GetConnection(account))
                using (GitHttpClient client = await connection.GetClientAsync<GitHttpClient>())
                {
                    // Capture the currentUserId so it can be used to filter PR's later.
                    //
                    Guid userId = connection.AuthorizedIdentity.Id;

                    await foreach (var pr in FetchPullRequests(client, userId, account, state))
                    {
                        // Create a connection to the AzureDevOps Git API.
                        //
                        {
                            var commits = await client.GetPullRequestCommitsAsync(pr.Repository.Id, pr.PullRequestId);
                            pr.Commits = commits.ToArray();

                            yield return new PullRequestViewElement(pr, account.Handler!);
                        }
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
            await foreach (var pr in FetchAccountActivePullRequsts(client, userId, accountConfig))
            {
                // Hack to not display drafts for now.
                //
                if (pr.IsDraft == true)
                {
                    m_statistics.Drafts++;

                    if (state == PrState.Drafts)
                    {
                        yield return pr;
                    }
                    else
                    {
                        continue;
                    }
                }

                // Try to find our selves in the reviewer list.
                //
                if (!TryGetReviewer(pr, userId, out IdentityRefWithVote reviewer))
                {
                    //  Skip this review if we aren't assigned.
                    //
                    continue;
                }

                // If we have already casted a "final" vote, then skip it.
                //
                if (reviewer.HasFinalVoteBeenCast())
                {
                    m_statistics.SignedOff++;
                    continue;
                }

                // TODO: It would be nice if there was a way to tell if
                // the review was changed since you started waiting.
                //
                if (reviewer.IsWaiting())
                {
                    m_statistics.Waiting++;

                    if (state == PrState.Waiting)
                    {
                        yield return pr;
                    }
                }
                else
                {
                    // If these criteria haven't been met, then the PR is actionable.
                    //
                    m_statistics.Actionable++;

                    if (state == PrState.Actionable)
                    {
                        yield return pr;
                    }
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

            List<GitPullRequest> requests = await client.GetPullRequestsAsync(accountConfig.Project, accountConfig.RepoName, criteria);
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
        private static VssConnection GetConnection(AccountConfig account)
        {
            VssCredentials credential;

            // If the user didn't configure a PAT token, try to login via AAD.
            //
            if (account.PersonalAccessToken == null)
            {
                 credential = new VssAadCredential(UserPrincipal.Current.EmailAddress);
            }
            else
            {
                credential = new VssBasicCredential(string.Empty, account.PersonalAccessToken);
            }

            return new VssConnection(account.OrganizationUrl, credential);
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

            EventHandler<StatisticsUpdateEventArgs>? handler = StatisticsUpdate;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
    }
}
