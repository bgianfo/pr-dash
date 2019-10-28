using System;
using System.Collections.Generic;
using System.Linq;
using PrDash.DataSource;
using PrDash.Configuration;
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
        /// <exception cref="ArgumentNullException"><paramref name="config"/> is <c>null</c>.</exception>
        public static void RunUiLoop(Config config, IPullRequestSource source)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Application.Init();

            Application.Current.ColorScheme = CustomColorSchemes.Main;

            var contentWindow = new Window("Pull Requests To Review:")
            {
                Width = Dim.Fill(),
                Height = config.StatusBarEnabled ? Dim.Fill() - Dim.Sized(3) : Dim.Fill(),
                ColorScheme = CustomColorSchemes.MutedEdges,
            };

            PullRequestView reView = new PullRequestView(source);
            contentWindow.Add(reView);

            Application.Top.Add(contentWindow);

            if (config.StatusBarEnabled)
            {
                StatusBar status = new StatusBar(source);
                var statusWindow = new Window("Status:")
                {
                    Width = Dim.Fill(),
                    Height = Dim.Sized(3),
                    Y = Pos.Bottom(contentWindow),
                    ColorScheme = CustomColorSchemes.MutedEdges,
                };
                statusWindow.Add(status);
                Application.Top.Add(statusWindow);
            }

            Application.Run();
        }
    }
}
