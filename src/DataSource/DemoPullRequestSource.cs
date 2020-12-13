using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
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
#pragma warning disable CA5394 // Do not use insecure randomness

        /// <summary>
        /// Class to generate random date times within a bound.
        /// </summary>
        internal class RandomDateTime
        {
            private static DateTime m_start = new DateTime(2019, 1, 1);
            private Random m_gen = new Random();
            private int m_range = (DateTime.Today - m_start).Days;

            public DateTime Next()
            {
                return m_start.AddDays(m_gen.Next(m_range)).AddHours(m_gen.Next(0, 24)).AddMinutes(m_gen.Next(0, 60)).AddSeconds(m_gen.Next(0, 60));
            }
        }

#pragma warning restore CA5394 // Do not use insecure randomness

        private RandomDateTime m_dateTimeGen = new RandomDateTime();

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
            yield return Fake("Docs: Document AAD feature in README.md");

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

        private PullRequestViewElement Fake(string title, string name = "Brian Gianforcaro")
        {
            GitPullRequest pr = new GitPullRequest
            {
                CreatedBy = new IdentityRef() { DisplayName = name },
                Title = title,
                ArtifactId = $"vstsid://{Guid.NewGuid()}",
                CreationDate = m_dateTimeGen.Next(),
            };

            m_statistics.Actionable++;

            return new PullRequestViewElement(pr, new DemoPullRequestHandler());
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
