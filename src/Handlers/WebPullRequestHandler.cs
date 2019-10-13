using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Use xdg-open to open the users configured browser on Linux.
                //
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "/usr/bin/xdg-open",
                    Arguments = $"{url}",
                    UseShellExecute = false,

                    // Redirect std-out and std-error, as lots of browsers spew
                    // to console when launching a new instance / tab.
                    //
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                Process.Start(startInfo);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // This is the best way to users configured browser on windows AFAIK?
                //
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else
            {
                throw new NotSupportedException("Opening the URL from this operating system is not yet supported.");
            }
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
