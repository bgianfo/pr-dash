using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Terminal.Gui;

namespace PrDash.View
{
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "This isn't a collection")]
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "No need to implement more enumerator")]
    public class PullRequestView : ListView
    {
        private readonly IList m_backingList;

        public PullRequestView(IList source)
            : base(source)
        {
            AllowsMarking = true;

            m_backingList = source;

            // Override the color scheme to our main theme for this view.
            //
            ColorScheme = CustomColorSchemes.Main;
        }

        private void HandleSelectedPullRequest(int selectedIndex)
        {
            PullRequestViewElement element = (PullRequestViewElement)m_backingList[selectedIndex];

            element.InvokeHandler();
        }

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
                    HandleSelectedPullRequest(SelectedItem);
                    return true;
            }

            // Forward everything else to the real implementation.
            //
            return base.ProcessKey(keyEvent);
        }
    }
}