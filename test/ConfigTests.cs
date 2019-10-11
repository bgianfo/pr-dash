using PrDash;
using System.IO;
using Xunit;
using PrDash.Configuration;

namespace PrDash.Tests
{
    /// <summary>
    /// Test cases for validating the parsing functionality in the <see cref="Config"/> class.
    /// </summary>
    public class ConfigTests
    {
        /// <summary>
        /// Example Yaml configuration document.
        /// </summary>
        private static string ExampleConfigDocument = @"
            accounts:
              - project_name: test1
                org_url: http://example1.com
                pat:   abc123

              - project_name: test2
                org_url: http://example2.com
                pat:   321cba
            ";

        /// <summary>
        /// Testing loading Yaml configuration from an in memory string.
        /// </summary>
        [Fact]
        public void TestParsingInMemoryYamlConfig()
        {
            // Load the yaml document.
            //
            Config config = Config.FromString(ExampleConfigDocument);

            // We should have some valid accounts at this point.
            //
            Assert.NotEmpty(config.Accounts);
            Assert.Equal(2, config.Accounts.Count);

            // We should have unique values in our two account entries.
            //
            Assert.Equal("test1", config.Accounts[0].Project);
            Assert.Equal("test2", config.Accounts[1].Project);
        }

        /// <summary>
        /// Testing loading Yaml configuration from a file on disk.
        /// </summary>
        [Fact]
        public void TestParsingOnDiskYamlConfig()
        {
            string tempConfig = CreateTemporaryConfigFile();

            try
            {
                // Load the yaml document.
                //
                Config config = Config.FromFile(tempConfig);

                // We should have some valid accounts at this point.
                //
                Assert.NotEmpty(config.Accounts);
                Assert.Equal(2, config.Accounts.Count);

                // We should have unique values in our two account entries.
                //
                Assert.Equal("test1", config.Accounts[0].Project);
                Assert.Equal("test2", config.Accounts[1].Project);
            }
            finally
            {
                File.Delete(tempConfig);
            }
        }

        /// <summary>
        /// Utility function to create a temporary Yaml configuration file.
        /// </summary>
        private static string CreateTemporaryConfigFile()
        {
            // Get the full name of the newly created Temporary file.
            //
            // Note: that the GetTempFileName() method actually creates
            // a 0-byte file and returns the name of the created file.
            //
            string fileName = Path.GetTempFileName();

            // Create a FileInfo object to set the file's attributes
            //
            FileInfo fileInfo = new FileInfo(fileName);

            // Set the Attribute property of this file to Temporary.
            // Although this is not completely necessary, the .NET Framework is able
            // to optimize the use of Temporary files by keeping them cached in memory.
            //
            fileInfo.Attributes = FileAttributes.Temporary;

            // Write the example configuration to disk.
            //
            using (StreamWriter sw = fileInfo.AppendText())
            {
                sw.Write(ExampleConfigDocument);
            }

            return fileName;
        }
    }
}
