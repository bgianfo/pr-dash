using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using PrDash.View;

namespace PrDash.DataSource
{
    /// <summary>
    /// Event arguments for callback on statistics updates.
    /// </summary>
    public class StatisticsUpdateEventArgs : EventArgs
    {
        public PullRequestStatistics? Statistics { get; set; }
    }

    public enum PrState
    {
        // Return only actionable pull requests.
        Actionable,

        // Return pull request marked as drafts.
        Drafts,

        // Return pull request we are waiting on.
        Waiting,

        // Return pull request we signed off on.
        SignedOff,

        // Return pull requests we created.
        Created,
    }

    /// <summary>
    /// Interface for interacting with the pull request.
    /// </summary>
    public interface IPullRequestSource
    {
        /// <summary>
        /// Event to list on statistics updates.
        /// </summary>
        event EventHandler<StatisticsUpdateEventArgs> StatisticsUpdate;

        /// <summary>
        /// Retrieves pull requests from the configured data source matching the given filter.
        /// </summary>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        IAsyncEnumerable<PullRequestViewElement> FetchAssignedPullRequests(PrState state);

        /// <summary>
        /// Retrieves all active pull requests this user has created.
        /// </summary>
        /// <returns>A stream of <see cref="GitPullRequest"/></returns>
        IAsyncEnumerable<PullRequestViewElement> FetchCreatedPullRequests();
    }
}
