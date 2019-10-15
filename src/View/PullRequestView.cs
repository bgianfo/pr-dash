using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// Custom ListView implementation that to support custom key processing.
    /// </summary>
    /// <seealso cref="Terminal.Gui.ListView" />
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "This isn't a collection")]
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "No need to implement more enumerator")]
    public class PullRequestView : ListView
    {
        /// <summary>
        /// The list of elements that this view is currently rendering.
        /// </summary>
        private readonly List<PullRequestViewElement> m_backingList;

        /// <summary>
        /// Initializes a new instance of the <see cref="PullRequestView"/> class.
        /// </summary>
        /// <param name="source">The source of elements for this view.</param>
        public PullRequestView(List<PullRequestViewElement> source)
            : base(source)
        {
            m_backingList = source;

            // Override the color scheme to our main theme for this view.
            //
            ColorScheme = CustomColorSchemes.Main;
        }

        /// <summary>
        /// Implement custom key handling on this view.
        /// </summary>
        /// <param name="keyEvent">Contains the details about the key that produced the event.</param>
        /// <returns>
        /// True if the key was handled, False otherwise.
        /// </returns>
        public override bool ProcessKey(KeyEvent keyEvent)
        {
            // Handle specific characters we want to have behavior.
            //
            char keyChar = (char)((uint)keyEvent.Key & (uint)(Key.CharMask));
            switch (keyChar)
            {
                // Map vim Keys to be up and down.
                //
                case 'j':
                    keyEvent.Key = Key.CursorDown;
                    return base.ProcessKey(keyEvent);
                case 'k':
                    keyEvent.Key = Key.CursorUp;
                    return base.ProcessKey(keyEvent);
                case 'q':
                    Application.RequestStop();
                    return true;
            }

            // Handle special characters.
            //
            switch (keyEvent.Key)
            {
                // Hook Control-C and Esc as exit.
                //
                case Key.Esc:
                case Key.ControlC:
                    Application.RequestStop();
                    return true;

                // Hook Enter to open the given pull request under the cursor.
                //
                case Key.Enter:
                    m_backingList[SelectedItem].InvokeHandler();
                    return true;
            }

            // Forward everything else to the real implementation.
            //
            return base.ProcessKey(keyEvent);
        }
    }
}