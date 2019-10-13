using System.Collections;
using System.Linq;
using PrDash.DataSource;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// The display class encapsulates the rendering of the console UI.
    /// </summary>
    public sealed class Display
    {
        /// <summary>
        /// Local reference to the backing pull request data source.
        /// </summary>
        private readonly IPullRequestSource m_pullRequestSource;

        /// <summary>
        /// Constructs a display.
        /// </summary>
        /// <param name="pullRequestSource">The backing data source to render from.</param>
        public Display(IPullRequestSource pullRequestSource)
        {
            m_pullRequestSource = pullRequestSource;
        }

        /// <summary>
        /// Populate the list of pull requests from the data source.
        /// </summary>
        /// <returns>A list of pull request content.</returns>
        private IList FetchPrData()
        {
            return m_pullRequestSource.FetchActivePullRequsts().ToList();
        }

        /// <summary>
        /// Initialize and run the UI main loop.
        /// </summary>
        public void RunUiLoop()
        {
            Application.Init();

            var contentWindow = new Window("Pull Requests To Review:")
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = CustomColorSchemes.MutedEdges,
            };

            contentWindow.Add(new PullRequestView(FetchPrData()));

            Application.Top.Add(contentWindow);

            Application.Run();
        }
    }
}
