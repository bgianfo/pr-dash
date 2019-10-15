using PrDash.Configuration;
using PrDash.DataSource;
using PrDash.View;

namespace PrDash
{
    /// <summary>
    /// The class entry point for the program.
    /// </summary>
    public static class EntryPoint
    {
        /// <summary>
        /// The program entry point for pr-dash.
        /// </summary>
        public static void Main()
        {
            Config.ValidateConfigExists();
            Config config = Config.FromConfigFile();

            AzureDevOpsPullRequestSource source = new AzureDevOpsPullRequestSource(config);

            Display.RunUiLoop(source);
        }
    }
}
