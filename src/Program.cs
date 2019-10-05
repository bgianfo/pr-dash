using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace PrDash
{
    public static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                // Organization URL, for example: https://dev.azure.com/fabrikam
                //
                Uri orgUrl = new Uri(args[0]);

                // See https://docs.microsoft.com/azure/devops/integrate/get-started/authentication/pats
                //
                string personalAccessToken = args[1];

                // The project to view.
                //
                string project = args[2];

                // Create a connection
                //
                using (VssConnection connection = GetConnection(orgUrl, personalAccessToken))
                {
                    // Show details a work item
                    //
                    ShowPullRequsts(connection, project).Wait();
                }
            }
            else
            {
                Console.WriteLine("Usage: pr-dash {org-url} {pat-token} {project-name}");
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

        private static IEnumerable<GitPullRequest> FilterRequests(IEnumerable<GitPullRequest> requests)
        {
            return requests;
        }

        private static VssConnection GetConnection(Uri orgUrl, string personalAccessToken)
        {
            // Create a connection
            //
            return new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalAccessToken));
        }
    }
}
