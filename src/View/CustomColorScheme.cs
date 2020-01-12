using System.Diagnostics.CodeAnalysis;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// Definitions of the color schemes used within the application.
    /// </summary>
    public static class CustomColorSchemes
    {
        /// <summary>
        /// The default back ground color for all widgets.
        /// </summary>
        private const Color BackGround = Color.Black;

        /// <summary>
        /// The default foreground color for all focused widgets.
        /// </summary>
        private const Color FocusedForGround = Color.BrightGreen;

        /// <summary>
        /// The main color scheme for the application.
        /// </summary>
        [SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "Don't Care")]
        public static ColorScheme Main = new ColorScheme()
        {
            Normal = Attribute.Make(Color.White, BackGround),
            Focus = Attribute.Make(FocusedForGround, BackGround),
        };

        /// <summary>
        /// The color scheme for the boarder/edges of the user interface.
        /// </summary>
        [SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "Don't Care")]
        public static ColorScheme MutedEdges = new ColorScheme()
        {
            Normal = Attribute.Make(Color.DarkGray, BackGround),
            Focus = Attribute.Make(FocusedForGround, BackGround),
        };
    }
}
