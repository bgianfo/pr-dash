using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using PrDash.DataSource;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// A view which represents the status bar of the application.
    /// </summary>
    /// <seealso cref="Terminal.Gui.View" />
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "This isn't a collection")]
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "No need to implement more enumerator")]
    public class StatusBar : Terminal.Gui.View
    {
        /// <summary>
        /// The label that holds the status bar contents.
        /// </summary>
        private readonly Label m_status;

        /// <summary>
        /// The data source the status bar pull data from.
        /// </summary>
        private readonly IPullRequestSource m_source;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        public StatusBar(IPullRequestSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            CanFocus = false;
            Height = 1;
            ColorScheme = CustomColorSchemes.MutedEdges;

            // Listen on statistics update callback.
            //
            m_source = source;
            m_source.StatisticsUpdate += StatisUpdateCallback;

            m_status = new Label("Loading...");

            Add(m_status);
        }

        private void StatisUpdateCallback(object sender, StatisticsUpdateEventArgs eventArgs)
        {
            PullRequestStatistics stats = eventArgs.Statistics;

            m_status.Text = $"Actionable: {stats.Actionable} | Waiting: {stats.Waiting} | SignedOff: {stats.SignedOff} | Drafts: {stats.Drafts}";
        }
    }
}
