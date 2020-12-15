using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// A view which displays the help and keyboard shortcuts for this application.
    /// </summary>
    /// <seealso cref="Dialog" />
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "This isn't a collection")]
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "No need to implement more enumerator")]
    public class HelpDialog : Dialog
    {
        /// <summary>
        /// The help dialog content to display.
        /// </summary>
        /// <remarks>
        /// Keep this in sync with the docs in the README.md
        /// </remarks>
        private static string HelpText =
@"Action Keys:
 h - Display this help dialog.
 r - Force refresh the current PR view.
 a - Switch the current view to actionable PRs.
 c - Switch the current view to created PRs.
 d - Switch the current view to draft PRs.
 s - Switch the current view to signed off PRs.
 w - Switch the current view to waiting PRs.
 q - Quit the program.
 Enter - Open the currently selected PR.

Movement Keys:
 ↑ - Select one pull request up.
 ↓ - Select one pull request down.
 k - Select one pull request up.
 j - Select one pull request down.

Mouse:
 Scroll Up   - Select one pull request up.
 Scroll Down - Select one pull request down.
 Left Click  - Open the currently selected PR.";

        /// <summary>
        /// The calculated width of the dialog contents.
        /// </summary>
        private static int ContentWidth => HelpText.Split("\n").Max(s => s.Length);

        /// <summary>
        /// The calculated height of the dialog contents.
        /// </summary>
        private static int ContentHeight => HelpText.Split("\n").Length;

        /// <summary>
        /// The calculated rectangle of the dialog contents.
        /// </summary>
        private static Rect ContentDimensions => new Rect(0, 0, ContentWidth, ContentHeight);

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Base Type Disposes")]
        public HelpDialog() : base("Help", ContentWidth + 5, ContentHeight + 5)
        {
            ColorScheme = CustomColorSchemes.Main;

            TextView text = new TextView(ContentDimensions)
            {
                Text = HelpText,
                ReadOnly = true,
            };

            Button button = new Button("Back", is_default: true)
            {
                Y = Pos.Bottom(text),
                X = Pos.Center(),
            };
            button.Clicked += () => Running = false;

            Add(button);
            Add(text);
        }

        /// <summary>
        /// Run the modal help dialog until the user exits.
        /// </summary>
        public static void Launch()
        {
            using (HelpDialog dialog = new HelpDialog())
            {
                Application.Run(dialog);
            }
        }
    }
}
