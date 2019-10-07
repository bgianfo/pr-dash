using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace PrDash
{
    /// <summary>
    /// The class entrypoint for the program.
    /// </summary>
    public static class EntryPoint
    {
        /// <summary>
        /// The program entrypoint for pr-dash.
        /// </summary>
        public static void Main()
        {
            Demo.UiMain();

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
            using (GitHttpClient client = connection.GetClient<GitHttpClient>())
            {
                GitPullRequestSearchCriteria criteria = new GitPullRequestSearchCriteria();
                criteria.Status = PullRequestStatus.Active;
                criteria.IncludeLinks = true;

                IEnumerable<GitPullRequest> pullRequests = await client.GetPullRequestsAsync(project, project, criteria);

                foreach (GitPullRequest request in pullRequests)
                {
                    Console.WriteLine("{0} by {1}", request.Title, request.CreatedBy.DisplayName);
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

        /// <summary>
        /// Validates that the required configuration file exists, terminates otherwise.
        /// </summary>
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
