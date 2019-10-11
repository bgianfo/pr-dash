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
                // Show details a work item
                //
                foreach (var pr in FetchAccountActivePullRequsts(account))
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

        private static IEnumerable<GitPullRequest> FetchAccountActivePullRequsts(AccountConfig accountConfig)
        {
            // Create a connection to the AzureDevOps git API.
            //
            using (VssConnection connection = GetConnection(accountConfig))
            using (GitHttpClient client = connection.GetClient<GitHttpClient>())
            {
                GitPullRequestSearchCriteria criteria = new GitPullRequestSearchCriteria();
                criteria.Status = PullRequestStatus.Active;
                criteria.IncludeLinks = true;

                IEnumerable<GitPullRequest> pullRequests = client.GetPullRequestsAsync(accountConfig.Project, accountConfig.Project, criteria).Result;
                return pullRequests;
            }
        }
    }
}
