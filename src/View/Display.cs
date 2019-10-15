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

            Application.Current.ColorScheme = CustomColorSchemes.Main;

            var contentWindow = new Window("Pull Requests To Review:")
            {
                Width = Dim.Fill(),
                Height = Dim.Fill() - Dim.Sized(3),
                ColorScheme = CustomColorSchemes.MutedEdges,
            };

            PullRequestView reView = new PullRequestView(source);
            contentWindow.Add(reView);

            StatusBar status = new StatusBar();
            var statusWindow = new Window("Status:")
            {
                Width = Dim.Fill(),
                Height = Dim.Sized(3),
                Y = Pos.Bottom(contentWindow),
                ColorScheme = CustomColorSchemes.MutedEdges,
            };

            statusWindow.Add(status);

            Application.Top.Add(contentWindow);
            Application.Top.Add(statusWindow);

            Application.Run();
        }
    }
}
