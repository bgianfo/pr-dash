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
        /// Hidden demo mode option.
        /// </summary>
        [Option('d', "demo-mode", Required = false, HelpText = "Run in demo mode", Hidden = true)]
        public bool DemoMode { get; set; }
    }
}
