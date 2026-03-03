using System;
using System.Xml.Linq;

namespace XamlToHtmlConverter.Parsing
{
    /// <summary>
    /// Provides helper methods for inspecting and printing XML DOM structure.
    /// Primarily used for debugging and structure visualization.
    /// </summary>
    public static class XmlDomInspector
    {
        /// <summary>
        /// Recursively prints an XML element, its attributes, and child nodes
        /// with indentation representing hierarchy depth.
        /// </summary>
        public static void Print(XElement element, int indent = 0)
        {
            var indentation = new string(' ', indent);

   

            foreach (var attr in element.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                    continue;

              
            }

            foreach (var node in element.Nodes())
            {
                if (node is XElement childElement)
                {
                    Print(childElement, indent + 2);
                }
                else if (node is XText textNode)
                {
                    var text = textNode.Value.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                       
                    }
                }
            }
        }
    }
}
