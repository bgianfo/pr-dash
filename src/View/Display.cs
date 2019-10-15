using System.Collections.Generic;
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
        /// Initialize and run the UI main loop.
        /// </summary>
        /// <param name="source">The backing data source to render from.</param>
        public static void RunUiLoop(IPullRequestSource source)
        {
            Application.Init();

            var contentWindow = new Window("Pull Requests To Review:")
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = CustomColorSchemes.MutedEdges,
            };

            contentWindow.Add(new PullRequestView(source));

            Application.Top.Add(contentWindow);


            Application.Run();
        }
    }
}
