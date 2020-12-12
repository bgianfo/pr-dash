using System;
using PrDash.Configuration;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace PrDash.Tests
{
    public class YamlExtensionTests
    {
        /// <summary>
        /// Tests the get string extension method in <see cref="YamlExtension"/>.
        /// </summary>
        [Fact]
        public void TestGetStringExtensionOnNullNode()
        {
            YamlMappingNode mappingNode = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                mappingNode.GetString("nonExistantField");
            });
        }

        /// <summary>
        /// Tests the get Uri extension method in <see cref="YamlExtension"/>.
        /// </summary>
        [Fact]
        public void TestGetUriExtensionOnNullNode()
        {
            YamlMappingNode mappingNode = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                mappingNode.GetUri("nonExistantField");
            });
        }
    }
}
