using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace PrDash
{
    public static class Program
    {
        static void Main()
        {
            ValidateConfigExists();

            Config config = Config.FromConfigFile();

            foreach (AccountConfig account in config.Accounts)
            {
                // Create a connection
                //
                using (VssConnection connection = GetConnection(account))
                {
                    // Show details a work item
                    //
                    ShowPullRequsts(connection, account.Project).Wait();
                }
            }
        }

        private static async Task ShowPullRequsts(VssConnection connection, string project)
        {
            GitHttpClient client = connection.GetClient<GitHttpClient>();

            GitPullRequestSearchCriteria criteria = new GitPullRequestSearchCriteria();
            criteria.Status = PullRequestStatus.Active;
            criteria.IncludeLinks = true;

            IEnumerable<GitPullRequest> pullRequests = await client.GetPullRequestsAsync(project, project, criteria);

            foreach (GitPullRequest request in pullRequests)
            {
                Console.WriteLine("{0} by {1}", request.Title, request.CreatedBy.DisplayName);
            }
        }

        private static VssConnection GetConnection(AccountConfig account)
        {
            // Create a connection
            //
            return new VssConnection(
                account.OrganizationUrl,
                new VssBasicCredential(string.Empty, account.PersonalAccessToken));
        }

        private static void ValidateConfigExists()
        {
            string configPath = Config.ConfigPath;

            if (!File.Exists(configPath))
            {
                Console.WriteLine("Configuration does not exist: {0}", configPath);

                Environment.Exit(1);
            }
        }
    }
}
