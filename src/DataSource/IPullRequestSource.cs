using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.DataSource
{
    /// <summary>
    /// Interface for interacting with the pull requst.
    /// </summary>
    public interface IPullRequestSource
    {
        /// <summary>
        /// Retreives all active pull rquests to the configured data source.
        /// </summary>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        public IEnumerable<GitPullRequest> FetchActivePullRequsts();
    }
}
