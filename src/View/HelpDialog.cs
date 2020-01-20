using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// A view which displays the help and keyboard shortcuts for this applcation.
    /// </summary>
    /// <seealso cref="Terminal.Gui.Dialog" />
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "This isn't a collection")]
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "No need to implement more enumerator")]
    public class HelpDialog : Terminal.Gui.Dialog
    {
        /// <summary>
        /// The help dialog content to display.
        /// </summary>
        private static string HelpText =
@"Action Keys:
 r - Force refresh the current PR view.
 a - Switch the current view to actionable PRs.
 w - Switch the current view to waiting PRs.
 h - Display this help dialog.
 q - Quit the program.
 Enter - Open the currently selected PR.

Movement Keys:
 ↑ - Select one pull request up.
 ↓ - Select one pull request down.
 k - Select one pull request up.
 j - Select one pull request down.";

        /// <summary>
        /// The calculated width of the diaplog contents.
        /// </summary>
        private static int ContentWidth
        {
            get
            {
                return HelpText.Split("\n").Max(s => s.Length);
            }
        }

        /// <summary>
        /// The calculated height of the diaplog contents.
        /// </summary>
        private static int ContentHeight
        {
            get
            {
                return HelpText.Split("\n").Length;
            }
        }

        /// <summary>
        /// The calculated rectangle of the diaplog contents.
        /// </summary>
        private static Rect ContentDimensions
        {
            get
            {
                return new Rect(0, 0, ContentWidth, ContentHeight);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
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
                Clicked = () => { Running = false; },
                Y = Pos.Bottom(text),
                X = Pos.Center(),
            };

            Add(button);
            Add(text);
        }

        /// <summary>
        /// Run the modal help diallog until the user exits.
        /// </summary>
        public static void Launch()
        {
            HelpDialog dialog = new HelpDialog();
            Application.Run(dialog);
        }
    }
}
