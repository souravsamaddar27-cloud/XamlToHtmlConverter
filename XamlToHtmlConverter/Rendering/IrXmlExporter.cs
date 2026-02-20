using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Provides functionality to export IR elements
    /// back into an XML document structure.
    /// </summary>
    public static class IrXmlExporter
    {
        /// <summary>
        /// Converts the root IR element into an XDocument.
        /// </summary>
        public static XDocument Export(IrElement root)
        {
            var rootElement = ConvertToXElement(root);
            return new XDocument(rootElement);
        }
        /// <summary>
        /// Recursively converts an IR element into an XElement,
        /// including properties, text content, and children.
        /// </summary>
        private static XElement ConvertToXElement(IrElement element)
        {
            var xElement = new XElement(element.Type);

            //regular properties
            foreach (var prop in element.Properties)
            {
                xElement.Add(new XAttribute(prop.Key, prop.Value));
            }

            //Attached properties
            foreach (var attached in element.AttachedProperties)
            {
                xElement.Add(new XAttribute(attached.Key, attached.Value));
            }

            //Text content
            if (!string.IsNullOrWhiteSpace(element.InnerText))
            {
                xElement.Add(new XText(element.InnerText));
            }

            //Children
            foreach (var child in element.Children)
            {
                xElement.Add(ConvertToXElement(child));
            }
            return xElement;
        }
    }
}
