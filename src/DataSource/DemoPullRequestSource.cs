using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using PrDash.Handlers;
using PrDash.View;

namespace PrDash.DataSource
{
    /// <summary>
    /// An implementation of <see cref="IPullRequestSource"/> which can be used
    /// for demo's of this application.
    /// </summary>
    public class DemoPullRequestSource : IPullRequestSource
    {
        /// <summary>
        /// Statistics tracking
        /// </summary>
        private PullRequestStatistics m_statistics = new PullRequestStatistics();

        /// <summary>
        /// Event handler for receiving updates to the pull request statistics.
        /// </summary>
        public event EventHandler<StatisticsUpdateEventArgs>? StatisticsUpdate;

        /// <inheritdoc/>
        public async IAsyncEnumerable<PullRequestViewElement> FetchAssignedPullRequests(PrState state)
        {
            m_statistics.Reset();

            await Task.CompletedTask;

            yield return Fake("Fix: Validate function parameters coming from user", "Alice G.");
            yield return Fake("Docs: Use xunit branding from the website");
            yield return Fake("Docs: Update README.md with more details");
            yield return Fake("GitPullRequestExtensions: Fix null - ref bug where a PR can have no commits");
            yield return Fake("Feature: Add description view, disabled by default for now.");
            yield return Fake("DataSouce: Skip reviews that the reviewer has declined");
            yield return Fake("Docs: Try to syntax highlight config in README.md");
            yield return Fake("Docs: Update README.md");
            yield return Fake("Feature: Add 's' hot key to filter view to signedoff pull requests");
            yield return Fake("Switch build version");
            yield return Fake("Docs: Document AAD feature in README.md(35 hours ago)");

            m_statistics.SignedOff = 20;
            m_statistics.Waiting = 2;
            m_statistics.Drafts = 1;

            OnStatisticsUpdate();
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<PullRequestViewElement> FetchCreatedPullRequests()
        {
            await Task.CompletedTask;
            yield break;
        }

        private PullRequestViewElement Fake(string title, string name =  "Brian Gianforcaro")
        {
            GitPullRequest pr = new GitPullRequest
            {
                CreatedBy = new IdentityRef() { DisplayName = name },
                Title = title,
                ArtifactId = $"vstsid://{Guid.NewGuid()}",
                CreationDate = RandomDate(),
            };

            m_statistics.Actionable++;

            return new PullRequestViewElement(pr, new DemoPullRequestHandler());
        }

        /// <summary>
        /// Generates a random date.
        /// </summary>
        /// <returns></returns>
        private static DateTime RandomDate()
        {
            DateTime start = new DateTime(2020, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(new Random().Next(range));
        }

        /// <summary>
        /// Invokes event update when statistics are updated.
        /// </summary>
        private void OnStatisticsUpdate()
        {
            StatisticsUpdateEventArgs eventArgs = new StatisticsUpdateEventArgs()
            {
                Statistics = m_statistics
            };

            StatisticsUpdate?.Invoke(this, eventArgs);
        }
    }
}
