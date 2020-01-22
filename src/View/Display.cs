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
        /// Gets the title for the actionable pull requests view.
        /// </summary>
        public const string ActionableTitle = "Actionable Pull Requests To Review:";

        /// <summary>
        /// Gets the title for the waiting pull requests view.
        /// </summary>
        public const string WaitingTitle = "Waiting Pull Requests To Review:";

        /// <summary>
        /// Gets the height of the status bar.
        /// </summary>
        private static Dim StatusBarHeight
        {
            get { return Dim.Sized(3); }
        }

        /// <summary>
        /// Gets the default window theme.
        /// </summary>
        private static ColorScheme WindowTheme
        {
            get { return CustomColorSchemes.MutedEdges; }
        }

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
            Toplevel top = Application.Top;

            // We intentionally initialize the status bar first, as the status
            // bar hooks events on the source, and the pull request view, will
            // drive API calls on the source which will trigger those events.
            // To avoid races here, make sure to hook first, run later.
            //
            StatusBar statusBar = new StatusBar(source);
            PullRequestView requestView = new PullRequestView(source);

            Window contentWindow = new Window(ActionableTitle)
            {
                Width = Dim.Fill(),
                Height = config.StatusBarEnabled ? Dim.Fill() - StatusBarHeight : Dim.Fill(),
                ColorScheme = WindowTheme,
            };

            contentWindow.Add(requestView);
            top.Add(contentWindow);

            if (config.StatusBarEnabled)
            {
                Window statusWindow = new Window("Status:")
                {
                    Width = Dim.Fill(),
                    Height = StatusBarHeight,
                    Y = Pos.Bottom(contentWindow),
                    ColorScheme = WindowTheme,
                };

                statusWindow.Add(statusBar);
                top.Add(statusWindow);
            }

            Application.Run();
        }
    }
}
