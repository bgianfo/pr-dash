using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using Mono.Terminal;
using PrDash.DataSource;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// Custom ListView implementation that to support custom key processing.
    /// </summary>
    /// <seealso cref="Terminal.Gui.ListView" />
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "This isn't a collection")]
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "No need to implement more enumerator")]
    public partial class PullRequestView : ListView
    {
        /// <summary>
        /// The interval in which we should refresh this views data.
        /// </summary>
        private static TimeSpan RefreshTimerInterval = TimeSpan.FromMinutes(30);

        /// <summary>
        /// The contents of the view when the data is loading.
        /// </summary>
        private static List<string> LoadingContents = new List<string>()
        {
            " Loading..."
        };

        /// <summary>
        /// The pull request source data source used to refresh the view's state.
        /// </summary>
        private readonly IPullRequestSource m_pullRequestSource;

        /// <summary>
        /// The list of elements that this view is currently rendering.
        /// </summary>
        private List<PullRequestViewElement> m_backingData = new List<PullRequestViewElement>();

        /// <summary>
        /// The state we wish to view pull requests in.
        /// </summary>
        private PrState m_stateToView = PrState.Actionable;

        /// <summary>
        /// The state we wish to view pull requests in.
        /// </summary>
        private RefreshTask m_refreshTask;

        /// <summary>
        /// The view holding the PR description.
        /// </summary>
        private TextView? m_descriptionView;

        /// <summary>
        /// Current title text, for use in UI and in console title.
        /// </summary>
        private string m_titleText = Display.ActionableTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="PullRequestView"/> class.
        /// </summary>
        /// <param name="source">The source of elements for this view.</param>
        public PullRequestView(IPullRequestSource source, TextView? descriptionView)
            : base(LoadingContents)
        {
            m_pullRequestSource = source;
            m_descriptionView = descriptionView;

            // Override the color scheme to our main theme for this view.
            //
            ColorScheme = CustomColorSchemes.Main;

            if (m_descriptionView != null)
            {
                // Subscribe to selection change.
                //
                SelectedChanged += OnSelectedPullRequestChanged;
            }

            // Post request to the refresh task to populate this view.
            //
            m_refreshTask = RefreshTask.Create(this);

            SwitchPrStateView(PrState.Actionable);

            // Setup a timer to fire in the future to refresh again.
            //
            Application.MainLoop.AddTimeout(RefreshTimerInterval, RefreshTimerCallback);
        }

        /// <summary>
        /// Switch what pull requests we want to view.
        /// </summary>
        /// <param name="newState">What state we want the pull requests we are looking at to be in.</param>
        private void SwitchPrStateView(PrState newState)
        {
            m_stateToView = newState;

            switch (m_stateToView)
            {
                case PrState.Actionable:
                    m_titleText = Display.ActionableTitle;
                    break;
                case PrState.Created:
                    m_titleText = Display.CreatedTitle;
                    break;
                case PrState.Drafts:
                    m_titleText = Display.DraftsTitle;
                    break;
                case PrState.SignedOff:
                    m_titleText = Display.SignedOffTitle;
                    break;
                case PrState.Waiting:
                    m_titleText = Display.WaitingTitle;
                    break;
                default:
                    throw new NotSupportedException(m_stateToView.ToString());
            }

            // Update the title of the window when we switch.
            //
            Window? parent = Application.Top.Subviews.FirstOrDefault() as Window;

            if (parent != null)
            {
                parent.Title = m_titleText;
            }

            // Force refresh the contents.
            //
            m_refreshTask.RequestRefresh();
        }

        /// <summary>
        /// Implement custom key handling on this view.
        /// </summary>
        /// <param name="kb">Contains the details about the key that produced the event.</param>
        /// <returns>True if the key was handled, False otherwise.</returns>
        public override bool ProcessKey(KeyEvent kb)
        {
            if (kb == null)
            {
                throw new ArgumentNullException(nameof(kb));
            }

            // Handle specific characters we want to have behavior.
            //
            char keyChar = (char)((uint)kb.Key & (uint)Key.CharMask);
            switch (keyChar)
            {
                // Map vim Keys to be up and down.
                //
                case 'j':
                    kb.Key = Key.CursorDown;
                    return base.ProcessKey(kb);

                case 'k':
                    kb.Key = Key.CursorUp;
                    return base.ProcessKey(kb);

                // Enable hotkey refresh.
                //
                case 'r':
                    m_refreshTask.RequestRefresh();
                    return true;

                // Enable hotkey switch view to waiting.
                //
                case 'w':
                    SwitchPrStateView(PrState.Waiting);
                    return true;

                // Enable hotkey switch view to drafts.
                //
                case 'd':
                    SwitchPrStateView(PrState.Drafts);
                    return true;

                // Enable hotkey switch view to actionable.
                //
                case 'a':
                    SwitchPrStateView(PrState.Actionable);
                    return true;

                // Enable hotkey switch view to signed off.
                //
                case 's':
                    SwitchPrStateView(PrState.SignedOff);
                    return true;

                // Enable hotkey switch view to our created prs.
                //
                case 'c':
                    SwitchPrStateView(PrState.Created);
                    return true;

                // Enable hotkey help.
                //
                case 'h':
                    HelpDialog.Launch();
                    return true;

                // Enable hotkey quit.
                //
                case 'q':
                    Application.RequestStop();
                    return true;
            }

            // Handle special characters.
            //
            switch (kb.Key)
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
                    OnOpenSelectedItem();
                    return true;
            }

            // Forward everything else to the real implementation.
            //
            return base.ProcessKey(kb);
        }

        /// <summary>
        /// Implement custom mouse handling on this view.
        /// </summary>
        /// <param name="me">Contains the details about the mouse action that produced the event.</param>
        /// <returns>True if the event was handled, False otherwise.</returns>
        public override bool MouseEvent(MouseEvent me)
        {
            switch (me.Flags)
            {
                case MouseFlags.WheeledDown:
                    return base.ProcessKey(new KeyEvent { Key = Key.CursorDown });

                case MouseFlags.WheeledUp:
                    return base.ProcessKey(new KeyEvent { Key = Key.CursorUp });

                case MouseFlags.Button1Clicked:
                    OnOpenSelectedItem();
                    return true;
                default:
                    return base.MouseEvent(me);
            }
        }

        /// <summary>
        /// Handler for opening the selected pull request.
        /// </summary>
        private void OnOpenSelectedItem()
        {
            if (SelectedItem < m_backingData.Count)
            {
                // TODO: Figure out the right way to do this.
                //
#pragma warning disable EPC13

                Task.Run(() => m_backingData[SelectedItem].OpenPullRequest());

#pragma warning restore EPC13

            }
        }

        /// <summary>
        /// Selection change callback.
        /// </summary>
        private void OnSelectedPullRequestChanged()
        {
            if (SelectedItem > m_backingData.Count)
            {
                m_descriptionView!.Text = string.Empty;
                return;
            }

            // N.B. Implement basic word wrap, as it appears TextView's don't support it currently in the terminal library we use.
            //
            var description = m_backingData[SelectedItem].Description.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder builder = new StringBuilder();
            int actualWidth = 0;

            foreach (var chunk in description)
            {
                actualWidth += chunk.Length + 1;

                if (actualWidth > m_descriptionView!.Frame.Size.Width)
                {
                    builder.AppendLine();
                    actualWidth = chunk.Length + 1;
                }

                builder.Append(chunk);
                builder.Append(' ');
            }

            m_descriptionView!.Text = builder.ToString();
        }

        /// <summary>
        /// Timer callback to refresh the view on the timer interval.
        /// </summary>
        /// <param name="MainLoop">The main loop.</param>
        /// <returns>True if the timer should be re-added, false otherwise.</returns>
        private bool RefreshTimerCallback(MainLoop main)
        {
            m_refreshTask.RequestRefresh();
            return true;
        }

        /// <summary>
        /// Insert the requested pull request into the backing List directly in a sorted order.
        /// </summary>
        /// <param name="element">The element to insert.</param>
        private void InsertElementSorted([NotNull] PullRequestViewElement element)
        {
            // Implement sorted insertion to keep the list sorted as the UI is refreshed
            // with results that are being streamed back as they are received.
            //
            // List<T>.BinarySearch(...) is documented to return:
            //
            //   The zero-based index of item in the sorted List<T>, if item is found; otherwise,
            //   a negative number that is the bitwise complement of the index of the next element
            //   that is larger than item or, if there is no larger element, the bitwise complement of Count.
            //
            // As we know there will be no matching items in the list, we can use the  bitwise compliment
            // directly as the insertion index into the list.
            //
            var insertionIndex = m_backingData.BinarySearch(element);
            m_backingData.Insert(~insertionIndex, element);

            // Force the UI Loop to re-render this view.
            //
            SetSource(m_backingData);
            Application.Refresh();
        }

        /// <summary>
        /// Populate the list of pull requests from the data source.
        /// </summary>
        /// <returns>A list of pull request content.</returns>
        private async Task RefreshCallback()
        {
            try
            {
                await SetSourceAsync(LoadingContents);
                Application.Refresh();

                // Make sure to reset selected item before insert so it's never out of range.
                //
                SelectedItem = 0;

                // Clear the backing list, but don't re-render yet.
                //
                m_backingData.Clear();

                IAsyncEnumerable<PullRequestViewElement> pullRequests;

                if (m_stateToView == PrState.Created)
                {
                    pullRequests = m_pullRequestSource.FetchCreatedPullRequests();
                }
                else
                {
                    pullRequests = m_pullRequestSource.FetchAssignedPullRequests(m_stateToView);
                }

                await foreach (PullRequestViewElement element in pullRequests)
                {
                    InsertElementSorted(element);
                    Console.Title = $"{m_titleText} {m_backingData.Count}";
                }

                // Force the UI Loop to re-render empty to clear the loading text.
                //
                await SetSourceAsync(m_backingData);
                Application.Refresh();
                Console.Title = $"{m_titleText} {m_backingData.Count}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Application.RequestStop();
                Environment.Exit(1);
                throw;
            }
        }
    }
}
