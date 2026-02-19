using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Parsing
{
    public class XmlToIrConverterRecursive : IXmlToIrConverter
    {
        public IrElement Convert(XElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            return ConvertElement(element);
        }
        private IrElement ConvertElement(XElement element)
        {
            var ir = new IrElement(element.Name.LocalName);
            ProcessAttributes(element, ir);
            ProcessText(element, ir);
            ProcessChildren(element, ir);
            return ir;
        }

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

        private void ProcessText(XElement element,IrElement ir)
        {
            var text = string.Join(" ",element.Nodes().OfType<XText>().Select(t=>t.Value.Trim()).Where(t=>!string.IsNullOrWhiteSpace(t)));
            if (!string.IsNullOrWhiteSpace(text))
                ir.InnerText = text;
        }
        private void ProcessChildren(XElement element, IrElement ir)
        {
            foreach(var child in element.Elements())
            {
                ir.Children.Add(ConvertElement(child));
            }
        }
        private bool IsAttachedProperty(string name) {
            return name.Contains(".");
        }
    }
}