
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.View
{
    /// <summary>
    /// Represents a pull request element in the <see cref="PullRequestView"/>.
    /// </summary>
    /// <remarks>
    /// This exists purley so that we can customize the ToString implementation.
    /// </remarks>
    public class PullRequestViewElement
    {
        /// <summary>
        /// The pull request we are wrapping.
        /// </summary>
        private readonly GitPullRequest m_pullRequest;

        /// <summary>
        /// Constructs a new element which wraps a <see cref="GitPullRequest"/> object.
        /// </summary>
        /// <param name="request"></param>
        public PullRequestViewElement(GitPullRequest request)
        {
            m_pullRequest = request;
        }

        public GitPullRequest PullRequest { get { return m_pullRequest; } }

        /// <summary>
        /// Special ToString implementation that will be called by the ListView control when it renders each element.
        /// </summary>
        /// <returns>The string representation of the pull request.</returns>
        public override string ToString() => $"{m_pullRequest.Title} - {m_pullRequest.CreatedBy.DisplayName}".Trim();
    }
}