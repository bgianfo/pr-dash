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
        /// The title for the actionable pull requests view.
        /// </summary>
        public const string ActionableTitle = "Actionable PRs:";

        /// <summary>
        /// The title for the pull requests you created.
        /// </summary>
        public const string CreatedTitle = "Created PRs:";

        /// <summary>
        /// The title for the waiting pull requests view.
        /// </summary>
        public const string WaitingTitle = "Waiting PRs:";

        /// <summary>
        /// The title for the signed off pull requests view.
        /// </summary>
        public const string SignedOffTitle = "Signed Off PRs:";

        /// <summary>
        /// The title for the draft pull requests view.
        /// </summary>
        public const string DraftsTitle = "Draft PRs:";

        /// <summary>
        /// The height of the status bar.
        /// </summary>
        private static Dim StatusBarHeight => Dim.Sized(3);

        /// <summary>
        /// The height of the status bar.
        /// </summary>
        private static Dim DescriptionHeight => Dim.Sized(10);

        /// <summary>
        /// The default window theme.
        /// </summary>
        private static ColorScheme WindowTheme => CustomColorSchemes.MutedEdges;

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
            top.X = Pos.Center();
            top.Y = Pos.Center();
            top.Height = Dim.Fill();
            top.Width = Dim.Fill();

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
                    Height = Dim.Fill(),
                    Width = Dim.Fill(),
                    ReadOnly = true,
                };
            }

            using PullRequestView requestView = new PullRequestView(source, descriptionView);

            using Window contentWindow = new Window(ActionableTitle)
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

            // Start processing data within the view now that everything is constructed.
            //
            requestView.Start();

            Application.Run();
        }
    }
}
