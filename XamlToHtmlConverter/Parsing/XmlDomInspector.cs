using System;
using System.Xml.Linq;

namespace XamlToHtmlConverter.Parsing
{
    /// <summary>
    /// Utility class to inspect and print the XML DOM tree.
    /// Used for debugging and understanding structure.
    /// </summary>
    public static class XmlDomInspector
    {
        public static void Print(XElement element, int indent = 0)
        {
            var indentation = new string(' ', indent);

            Console.WriteLine($"{indentation}Element: {element.Name.LocalName}");

            foreach (var attr in element.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                    continue;

                Console.WriteLine($"{indentation}  Attribute: {attr.Name.LocalName} = {attr.Value}");
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
                        Console.WriteLine($"{indentation}  TextNode: {text}");
                    }
                }
            }
        }
    }
}
