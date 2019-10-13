using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using PrDash.Configuration;
using PrDash.View;

namespace PrDash.DataSource
{
    public class AzureDevOpsPullRequestSource : IPullRequestSource
    {
        private readonly Config m_config;

        public AzureDevOpsPullRequestSource(Config config)
        {
            m_config = config;
        }

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
    }
}
