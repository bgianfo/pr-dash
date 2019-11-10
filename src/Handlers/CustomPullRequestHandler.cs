using System;
using System.Diagnostics;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using PrDash.Configuration;

namespace PrDash.Handlers
{
    /// <summary>
    /// Handler which will open the selected pull request in a custom review UI.
    /// The protocol prefix is configurable by the user. This allows you so to
    /// associate your custom pull request viewer with a prefix, allowing us to
    /// launch the UI without knowing anything about your application.
    /// </summary>
    /// <seealso cref="IPullRequestHandler" />
    public class CustomPullRequestHandler : IPullRequestHandler
    {
        private readonly string m_protocolPrefix;
        private readonly AccountConfig m_config;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPullRequestHandler"/> class.
        /// </summary>
        /// <param name="accountConfig">The account configuration.</param>
        /// <param name="protocolPrefix">The protocol prefix.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="accountConfig"/> or
        /// <paramref name="protocolPrefix"/> is <c>null</c>.
        /// </exception>
        public CustomPullRequestHandler(AccountConfig accountConfig, string protocolPrefix)
        {
            if (accountConfig == null)
            {
                throw new ArgumentNullException(nameof(accountConfig));
            }

            if (string.IsNullOrEmpty(protocolPrefix))
            {
                throw new ArgumentNullException(nameof(protocolPrefix));
            }

            m_protocolPrefix = protocolPrefix;
            m_config = accountConfig;
        }

        /// <summary>
        /// Invokes the handler.
        /// </summary>
        /// <param name="pullRequest">The pull request to handle.</param>
        /// <exception cref="ArgumentNullException">pullRequest</exception>
        public void InvokeHandler(GitPullRequest pullRequest)
        {
            if (pullRequest == null)
            {
                throw new ArgumentNullException(nameof(pullRequest));
            }

            string url = ConstructUri(pullRequest);

            // Escape & characters so the shell doesn't try to interpret them.
            //
            url = url.Replace("&", "^&", StringComparison.InvariantCultureIgnoreCase);

            // This is the only way to get the protocol handler to be invoked AFAIK?
            //
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        /// <summary>
        /// Build a protocol handler specific compatible Url.
        /// </summary>
        /// <remarks>
        /// Example Url:
        /// protocol://open/?server=https%3A%2F%2Fteam.visualstudio.com%2F&project=myprojectl&repo=myrepo&pullRequest=10571&alert=true
        /// </remarks>
        private string ConstructUri(GitPullRequest pr)
        {
            string proj = m_config.Project;
            string url = $"{m_config.OrganizationUrl}&project={proj}&repo={proj}&pullRequest={pr.PullRequestId}&alert=true";

            // Some app Url decoders don't seem to support full url de-encoding... so just encode "enough".
            //
            url = url.Replace("/", "%2F", StringComparison.InvariantCultureIgnoreCase);
            url = url.Replace(":", "%3A", StringComparison.InvariantCultureIgnoreCase);

            return $"{m_protocolPrefix}://open/?server={url}";
        }
    }
}
