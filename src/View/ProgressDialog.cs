using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Client.Keychain.Logging;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// A view which displays the loading progress for this application.
    /// </summary>
    /// <seealso cref="Dialog" />
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "This isn't a collection")]
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "No need to implement more enumerator")]
    public class ProgressDialog : Dialog
    {
        /// <summary>
        /// The width of the progress contents.
        /// </summary>
        private static int ContentWidth = 20;

        /// <summary>
        /// The height of the dialog contents.
        /// </summary>
        private static int ContentHeight = 20;

        private Task m_progressBarTask;

        /// <summary>
        /// The calculated rectangle of the dialog contents.
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
        public ProgressDialog() : base("Loading", ContentWidth + 5, ContentHeight + 5)
        {
            ColorScheme = CustomColorSchemes.Main;

            ProgressBar bar = new ProgressBar(ContentDimensions);

            Add(bar);
        }

        internal void Run()
        {
            m_progressBarTask = Task.Run(() =>
            {
              Application.Run(this);
            });
        }

        /// <summary>
        /// Run the modal progress dialog until the user exits.
        /// </summary>
        public static ProgressDialog Launch()
        {
            ProgressDialog dialog = new ProgressDialog();
            dialog.Run();
            return dialog;
        }
    }
}
