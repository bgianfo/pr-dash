using CommandLine;

namespace PrDash.Configuration
{
    /// <summary>
    /// Represents the command line configuration for the dashboard.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// The verbose bar command line option.
        /// </summary>
        [Option('v', "verbose", Required = false, HelpText = "Sets output to verbose mode")]
        public bool Verbose { get; set; }

        /// <summary>
        /// The status bar command line option.
        /// </summary>
        [Option('s', "statusbar", Required = false, HelpText = "Enables status bar display, will be removed in the future.")]
        public bool StatusBarEnabled { get; set; }
    }
}
