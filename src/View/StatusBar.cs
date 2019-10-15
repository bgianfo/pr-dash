using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// A view which represents the status bar of the application.
    /// </summary>
    /// <seealso cref="Terminal.Gui.View" />
    public class StatusBar : Terminal.Gui.View
    {
        /// <summary>
        /// The label that holds the status bar contents.
        /// </summary>
        private readonly Label m_status;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        public StatusBar()
        {
            CanFocus = false;
            Height = 1;
            ColorScheme = CustomColorSchemes.MutedEdges;

            m_status = new Label("TODO!");

            Add(m_status);
        }
    }
}
