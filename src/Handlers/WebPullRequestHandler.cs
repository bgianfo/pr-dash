using System;
using System.Diagnostics;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using PrDash.Configuration;

namespace PrDash.Handlers
{
    /// <summary>
    /// Handler which will open the selected pull request in browser review UI.
    /// </summary>
    /// <seealso cref="IPullRequestHandler" />
    public class WebPullRequestHandler : IPullRequestHandler
    {
        /// <summary>
        /// The account configuration used to drive this handler.
        /// </summary>
        private readonly AccountConfig m_config;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebPullRequestHandler"/> class.
        /// </summary>
        /// <param name="accountConfig">The account configuration this handler will be driven with.</param>
        public WebPullRequestHandler(AccountConfig accountConfig)
        {
            m_config = accountConfig;
        }

        /// <summary>
        /// Invokes the handler.
        /// </summary>
        /// <param name="pullRequest">The pull request.</param>
        /// <exception cref="ArgumentNullException">pullRequest</exception>
        public void InvokeHandler(GitPullRequest pullRequest)
        {
            if (pullRequest == null)
            {
                throw new ArgumentNullException(nameof(pullRequest));
            }

            string url = ConstructUri(pullRequest);

            // This is the only way to get the protocol handler to be invoked AFAIK?
            //
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        /// <summary>
        /// Build a Url to the web view of the pull request.
        /// </summary>
        /// <remarks>
        /// Example Url: https://team.visualstudio.com/_git/myrepo/pullRequest/10571
        /// </remarks>
        private string ConstructUri(GitPullRequest pr)
        {
            return $"{m_config.OrganizationUrl}/_git/{m_config.Project}/pullrequest/{pr.PullRequestId}";
        }
    }
}
