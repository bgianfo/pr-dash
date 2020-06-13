using System;
using YamlDotNet.RepresentationModel;

namespace PrDash.Configuration
{
    /// <summary>
    /// Custom YamlDotNet extension methods.
    /// </summary>
    public static class YamlDotNetExtensions
    {
        /// <summary>
        /// Extracts a string value from the Yaml specified named scalar node.
        /// </summary>
        /// <param name="mappingNode">The root node that the scalar hangs off of.</param>
        /// <param name="scalarField">The name of the scalar node we want to extract the string value for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mappingNode"/> is <c>null</c>.</exception>
        public static string GetString(this YamlMappingNode mappingNode, string scalarField)
        {
            if (mappingNode == null)
            {
                throw new ArgumentNullException(nameof(mappingNode));
            }
#nullable disable warnings
            return ((YamlScalarNode)mappingNode.Children[new YamlScalarNode(scalarField)]).Value;
#nullable enable warnings
        }

        /// <summary>
        /// Extracts a Uri value from the Yaml specified named scalar node.
        /// </summary>
        /// <param name="mappingNode">The root node that the scalar hangs off of.</param>
        /// <param name="scalarField">The name of the scalar node we want to extract the Uri value for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mappingNode"/> is <c>null</c>.</exception>
        public static Uri GetUri(this YamlMappingNode mappingNode, string scalarField)
        {
            if (mappingNode == null)
            {
                throw new ArgumentNullException(nameof(mappingNode));
            }

            return new Uri(mappingNode.GetString(scalarField));
        }
    }
}
