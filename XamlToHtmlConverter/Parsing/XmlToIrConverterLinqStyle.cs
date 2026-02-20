using System;
using System.Linq;
using System.Xml.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Parsing
{
	/// <summary>
	/// Converts XML elements into IR elements using LINQ-based traversal.
	/// Maps attributes, text content, and child elements recursively.
	/// </summary>
	public class XmlToIrConverterLinqStyle : IXmlToIrConverter
	{
		/// <summary>
		/// Transforms the provided XElement into an IrElement.
		/// Processes attributes, inner text, and child elements.
		/// </summary>
		public IrElement Convert(XElement element)
		{
			if(element==null)
				throw new ArgumentNullException(nameof(element));

			var ir = new IrElement(element.Name.LocalName);

			/// <summary>
			/// Retrieves all non-namespace attributes from the XML element
			/// and maps them to regular or attached IR properties.
			/// </summary>
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
			/// <summary>
			/// Extracts, trims, and combines all non-empty text nodes
			/// into a single inner text value for the IR element.
			/// </summary>
			var text = element.Nodes().OfType<XText>().Select(t => t.Value.Trim()).Where(t => !string.IsNullOrWhiteSpace(t));
			var combined = string.Join(" ", text);
			if(!string.IsNullOrWhiteSpace(combined))
				ir.InnerText = combined;

			//children
			/// <summary>
			/// Recursively converts child XML elements into IR elements
			/// and assigns them as children of the current IR node.
			/// </summary>
			ir.Children = element.Elements().Select(Convert).ToList();
			return ir;
		}
	}

}