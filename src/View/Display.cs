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

            Window contentWindow = new Window("Actionable Pull Requests To Review:")
            {
                Width = Dim.Fill(),
                Height = config.StatusBarEnabled ? Dim.Fill() - StatusBarHeight : Dim.Fill(),
                ColorScheme = WindowTheme,
            };

            contentWindow.Add(new PullRequestView(source));
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

                statusWindow.Add(new StatusBar(source));
                top.Add(statusWindow);
            }

            Application.Run();
        }
    }
}
