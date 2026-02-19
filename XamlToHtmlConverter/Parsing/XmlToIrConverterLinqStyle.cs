using System;
using System.Linq;
using System.Xml.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Parsing
{
	public class XmlToIrConverterLinqStyle : IXmlToIrConverter
	{
		public IrElement Convert(XElement element)
		{
			if(element==null)
				throw new ArgumentNullException(nameof(element));

			var ir = new IrElement(element.Name.LocalName);

			var attributes = element.Attributes().Where(a => !a.IsNamespaceDeclaration);
			foreach (var attr in attributes) 
			{
				var name = attr.Name.LocalName;
				if (name.Contains("."))
					ir.AttachedProperties[name] = attr.Value;
				else
					ir.Properties[name] = attr.Value;
			}

			//Text
			var text = element.Nodes().OfType<XText>().Select(t => t.Value.Trim()).Where(t => !string.IsNullOrWhiteSpace(t));
			var combined = string.Join(" ", text);
			if(!string.IsNullOrWhiteSpace(combined))
				ir.InnerText = combined;

			//children
			ir.Children = element.Elements().Select(Convert).ToList();
			return ir;
		}
	}

}