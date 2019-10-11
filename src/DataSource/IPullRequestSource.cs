using System.Collections.Generic;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.DataSource
{
    /// <summary>
    /// Interface for interacting with the pull request.
    /// </summary>
    public interface IPullRequestSource
    {
        /// <summary>
        /// Retrieves all active pull requests to the configured data source.
        /// </summary>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        public IEnumerable<GitPullRequest> FetchActivePullRequsts();
    }
}
