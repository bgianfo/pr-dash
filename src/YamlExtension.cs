using System;
using YamlDotNet.RepresentationModel;

namespace PrDash
{
    public static class YamlDotNetExtensions
    {
        public static string GetString(this YamlMappingNode mappingNode, string field)
        {
            if (mappingNode == null)
            {
                throw new ArgumentNullException(nameof(mappingNode));
            }

            return ((YamlScalarNode)mappingNode.Children[new YamlScalarNode(field)]).Value;
        }

        /// <summary>
        /// Extracts the string value from the yaml scalar nodel configuration stream from the specified input.
        /// </summary>
        /// <param name="yamlReader">The input.</param>
        public static Uri GetUri(this YamlMappingNode mappingNode, string scalarField)
        {
            return new Uri(mappingNode.GetString(scalarField));
        }
    }
}
