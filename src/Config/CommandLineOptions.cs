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
    }
}
