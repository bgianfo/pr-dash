using System;
using System.Linq;
using PrDash.Configuration;
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
        /// Gets the title for the actionable pull requests view.
        /// </summary>
        public const string ActionableTitle = "Actionable PRs:";

        /// <summary>
        /// Gets the title for the pull requests you created.
        /// </summary>
        public const string CreatedTitle = "Created PRs:";

        /// <summary>
        /// Gets the title for the waiting pull requests view.
        /// </summary>
        public const string WaitingTitle = "Waiting PRs:";

        /// <summary>
        /// Gets the title for the signed off pull requests view.
        /// </summary>
        public const string SignedOffTitle = "Signed Off PRs:";

        /// <summary>
        /// Gets the title for the draft pull requests view.
        /// </summary>
        public const string DraftsTitle = "Draft PRs:";

        /// <summary>
        /// Gets the height of the status bar.
        /// </summary>
        private static Dim StatusBarHeight
        {
            get { return Dim.Sized(3); }
        }

        /// <summary>
        /// Gets the height of the status bar.
        /// </summary>
        private static Dim DescriptionHeight
        {
            get { return Dim.Sized(10); }
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
            Dim computedHeight = Dim.Sized(0);

            // We intentionally initialize the status bar first, as the status
            // bar hooks events on the source, and the pull request view, will
            // drive API calls on the source which will trigger those events.
            // To avoid races here, make sure to hook first, run later.
            //
            StatusBar? statusBar = null;
            if (config.StatusBarEnabled)
            {
                computedHeight += StatusBarHeight;
                statusBar = new StatusBar(source);
            }

            TextView? descriptionView = null;
            if (config.DescriptionEnabled)
            {
                computedHeight += DescriptionHeight;

                descriptionView = new TextView()
                {
                    ReadOnly = true,
                };
            }

            PullRequestView requestView = new PullRequestView(source, descriptionView);

            Window contentWindow = new Window(ActionableTitle)
            {
                Width = Dim.Fill(),
                Height = Dim.Fill() - computedHeight,
                ColorScheme = WindowTheme,
            };

            contentWindow.Add(requestView);
            top.Add(contentWindow);

            if (config.DescriptionEnabled)
            {
                Window descriptionWindow = new Window("Description:")
                {
                    Width = Dim.Fill(),
                    Height = DescriptionHeight,
                    Y = Pos.Bottom(contentWindow),
                    ColorScheme = WindowTheme,
                };

                descriptionWindow.Add(descriptionView);
                top.Add(descriptionWindow);
            }

            if (config.StatusBarEnabled)
            {
                Window statusWindow = new Window("Status:")
                {
                    Width = Dim.Fill(),
                    Height = StatusBarHeight,
                    Y = Pos.Bottom(top.Subviews.Last()),
                    ColorScheme = WindowTheme,
                };

                statusWindow.Add(statusBar);
                top.Add(statusWindow);
            }

            Application.Run();
        }
    }
}
