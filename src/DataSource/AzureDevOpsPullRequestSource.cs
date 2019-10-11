using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using PrDash.Configuration;
using PrDash.DataSource;

namespace PrDash.DataSource
{
    public class AzureDevOpsPullRequestSource : IPullRequestSource
    {
        private Config m_config;

        public AzureDevOpsPullRequestSource(Config config)
        {
            m_config = config;
        }

        public IEnumerable<GitPullRequest> FetchActivePullRequsts()
        {
            foreach (AccountConfig account in m_config.Accounts)
            {
                foreach (var pr in FetchActionablePullRequests(account))
                {
                    yield return pr;
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
            // Create a connection to the AzureDevOps git API.
            //
            using (VssConnection connection = GetConnection(accountConfig))
            using (GitHttpClient client = connection.GetClient<GitHttpClient>())
            {
                currentUserId = connection.AuthorizedIdentity.Id;
                GitPullRequestSearchCriteria criteria = new GitPullRequestSearchCriteria();
                criteria.Status = PullRequestStatus.Active;
                criteria.IncludeLinks = true;

                IEnumerable<GitPullRequest> pullRequests = client.GetPullRequestsAsync(accountConfig.Project, accountConfig.Project, criteria).Result;
                return pullRequests;
            }
        }

        private static IEnumerable<GitPullRequest> FetchActionablePullRequests(AccountConfig accountConfig)
        {
            Guid currentUserId;
            IEnumerable<GitPullRequest> source = FetchAccountActivePullRequsts(accountConfig, out currentUserId);

            foreach (GitPullRequest pr in source)
            {
                // Hack to not display drafts for now.
                //
                if (pr.IsDraft.Value)
                {
                    continue;
                }

                // Try to find our selves in the reviewer list.
                //
                IdentityRefWithVote reviewer;
                if (!TryGetReviewer(pr, currentUserId, out reviewer))
                {
                    //  SKip this review if we aren't assigned.
                    //
                    continue;
                }

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
                if (currentUserId.Equals(new Guid(r.Id)))
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
