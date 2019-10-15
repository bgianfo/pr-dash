using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.Handlers
{
    /// <summary>
    /// Interface for invoking an action on the selected pull request.
    /// </summary>
    public interface IPullRequestHandler
    {
        /// <summary>
        /// Invokes an action on the specified pull request action.
        /// </summary>
        /// <param name="pr">The pull request that was selected.</param>
        void InvokeHandler(GitPullRequest pr);
    }
}
