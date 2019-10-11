using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using PrDash.Configuration;
using PrDash.DataSource;

namespace PrDash
{
    /// <summary>
    /// The class entrypoint for the program.
    /// </summary>
    public static class EntryPoint
    {
        /// <summary>
        /// The program entrypoint for pr-dash.
        /// </summary>
        public static void Main()
        {

            ValidateConfigExists();

            Config config = Config.FromConfigFile();
            AzureDevOpsPullRequestSource pullRequestSource = new AzureDevOpsPullRequestSource(config);

            Display display = new Display(pullRequestSource);
            display.UiMain();
       }

        /// <summary>
        /// Validates that the required configuration file exists, terminates otherwise.
        /// </summary>
        private static void ValidateConfigExists()
        {
            string configPath = Config.ConfigPath;

            if (!File.Exists(configPath))
            {
                Console.WriteLine("Configuration does not exist: {0}", configPath);
                Environment.Exit(1);
            }
        }
    }
}
