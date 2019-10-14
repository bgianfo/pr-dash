using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using PrDash.Configuration;
using PrDash.View;

namespace PrDash.DataSource
{
    /// <summary>
    /// An implmeentation of <see cref="IPullRequestSource"/> which retreives the active
    /// pull requests for a user, from the Azure DevOps server.
    /// </summary>
    public class AzureDevOpsPullRequestSource : IPullRequestSource
    {
        /// <summary>
        /// The configuration that we shoudl use to connect to the backing store.
        /// </summary>
        private readonly Config m_config;

        /// <summary>
        /// Constructs a new request source.
        /// </summary>
        /// <param name="config">The config to driver the system.</param>
        public AzureDevOpsPullRequestSource(Config config)
        {
            m_config = config;
        }

        /// <summary>
        /// Retrieves all active & actionable pull requests to the configured data source.
        /// </summary>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        public IEnumerable<PullRequestViewElement> FetchActivePullRequsts()
        {
            foreach (AccountConfig account in m_config.Accounts)
            {
                foreach (var pr in FetchActionablePullRequests(account))
                {
                    yield return new PullRequestViewElement(pr, account.Handler);
                }
            }
        }

        /// <summary>
        /// Retrieves all active & actionable pull requests to the configured data source.
        /// </summary>
        /// <param name="accountConfig">The account to retreive the pull requests for.</param>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        private static IEnumerable<GitPullRequest> FetchActionablePullRequests(AccountConfig accountConfig)
        {
            foreach (GitPullRequest pr in FetchAccountActivePullRequsts(accountConfig, out Guid currentUserId))
            {
                // Hack to not display drafts for now.
                //
                if (pr.IsDraft == true)
                {
                    continue;
                }

                // Try to find our selves in the reviewer list.
                //
                if (!TryGetReviewer(pr, currentUserId, out IdentityRefWithVote reviewer))
                {
                    //  SKip this review if we aren't assigned.
                    //
                    continue;
                }

                // If we have already casted a "final" vote, then skip it.
                //
                if (reviewer.HasFinalVoteBeenCast())
                {
                    continue;
                }

                // TODO: It would be nice if there was a way to tell if
                // the review was changed since you started waiting.
                //
                if (reviewer.IsWaiting())
                {
                    continue;
                }

                // If  these criteria haven't been met, then display the review.
                //
                yield return pr;
            }
        }

        /// <summary>
        /// Retrieves all active & actionable pull requests for a specific account.
        /// </summary>
        /// <param name="accountConfig">The account to get the pull requests for.</param>
        /// <param name="currentUserId">An out paramter that receives the <see cref="Guid"/> of the current user.</param>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        private static IEnumerable<GitPullRequest> FetchAccountActivePullRequsts(AccountConfig accountConfig, out Guid currentUserId)
        {
            // Create a connection to the AzureDevOps Git API.
            //
            using (VssConnection connection = GetConnection(accountConfig))
            using (GitHttpClient client = connection.GetClient<GitHttpClient>())
            {
                // Capture the currentUserId so it can be used to filter PR's later.
                //
                currentUserId = connection.AuthorizedIdentity.Id;

                // Only fetch pull requests which are active, and assigned to this user.
                //
                GitPullRequestSearchCriteria criteria = new GitPullRequestSearchCriteria
                {
                    ReviewerId = currentUserId,
                    Status = PullRequestStatus.Active,
                };

                return client.GetPullRequestsAsync(accountConfig.Project, accountConfig.Project, criteria).Result;
            }
        }

        /// <summary>
        /// Tries to get the current users reviewer object from a pull request.
        /// </summary>
        /// <param name="pullRequest">The pull request we want to look our selves up in.</param>
        /// <param name="currentUserId">The <see cref="Guid"/> of our current user.</param>
        /// <param name="reviewer">Output  parameter that points to our own reviewer object.</param>
        /// <returns></returns>
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

            reviewer = null;
            return false;
        }

        /// <summary>
        /// Factory method for creating the connection.
        /// </summary>
        /// <param name="account">Account details to create the connection for.</param>
        /// <returns>A valid <see cref="VssConnection"/> for the given account.</returns>
        private static VssConnection GetConnection(AccountConfig account)
        {
            return new VssConnection(
                account.OrganizationUrl,
                new VssBasicCredential(string.Empty, account.PersonalAccessToken));
        }
    }
}
