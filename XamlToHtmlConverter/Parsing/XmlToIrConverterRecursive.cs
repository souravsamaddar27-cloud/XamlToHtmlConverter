using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Parsing
{
    /// <summary>
    /// Converts XML elements into IR elements using
    /// a structured recursive processing approach.
    /// Separates attribute, text, and child handling into dedicated methods.
    /// </summary>
    public class XmlToIrConverterRecursive : IXmlToIrConverter
    {
        /// <summary>
        /// Validates input and starts recursive conversion.
        /// </summary>
        public IrElement Convert(XElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            return ConvertElement(element);
        }
        /// <summary>
        /// Converts a single XElement into an IrElement
        /// and delegates processing steps.
        /// </summary>
        private IrElement ConvertElement(XElement element)
        {
            var ir = new IrElement(element.Name.LocalName);
            ProcessAttributes(element, ir);
            ProcessText(element, ir);
            ProcessChildren(element, ir);
            return ir;
        }

        /// <summary>
        /// Maps XML attributes to standard or attached IR properties.
        /// </summary>
        private void ProcessAttributes(XElement element, IrElement ir)
        {
            foreach(var attr in element.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                    continue;

                var name = attr.Name.LocalName;
                var value = attr.Value;

                if (IsAttachedProperty(name))
                    ir.AttachedProperties[name] = value;
                else
                    ir.Properties[name] = value;
            }
        }

        /// <summary>
        /// Extracts and assigns combined non-empty text nodes
        /// as the IR element's inner text.
        /// </summary>
        private void ProcessText(XElement element,IrElement ir)
        {
            var text = string.Join(" ",element.Nodes().OfType<XText>().Select(t=>t.Value.Trim()).Where(t=>!string.IsNullOrWhiteSpace(t)));
            if (!string.IsNullOrWhiteSpace(text))
                ir.InnerText = text;
        }

        /// <summary>
        /// Recursively converts child XML elements
        /// and adds them to the IR children collection.
        /// </summary>
        private void ProcessChildren(XElement element, IrElement ir)
        {
            foreach(var child in element.Elements())
            {
                ir.Children.Add(ConvertElement(child));
            }
        }

        /// <summary>
        /// Determines whether an attribute represents
        /// an attached property based on naming convention.
        /// </summary>
        private bool IsAttachedProperty(string name) {
            return name.Contains(".");
        }
    }
}