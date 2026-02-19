//using System;
//using System.IO;
//using System.Linq;
//using System.Xml.Linq;
//using XamlToHtmlConverter.IR;

//namespace XamlToHtmlConverter.Parsing
//{

//    /// <summary>
//    /// Responsible for parsing XAML files into a neutral IR tree.
//    /// </summary>
//    public class XamlParser
//    {
//        /// <summary>
//        /// Parses a XAML file into an IR tree.
//        /// </summary>
//        public IrElement Parse(string xamlPath)
//        {
//            if (string.IsNullOrEmpty(xamlPath))
//                throw new ArgumentException("XAML path cannot be null or empty.");

//            if (!File.Exists(xamlPath))
//                throw new FileNotFoundException("XAML file not found.", xamlPath);

//            var document = XDocument.Load(xamlPath);

//            if (document == null)
//                throw new InvalidOperationException("XAML file has no root element.");

//            return ConvertElement(document.Root);
//        }
//        /// <summary>
//        /// Recursively converts an XElement into an IrElement.
//        /// </summary>
//        private IrElement ConvertElement(XElement element)
//        {
//            var irElement = new IrElement(element.Name.LocalName);

//            foreach(var attribute in element.Attributes())
//            {
//                if (attribute.IsNamespaceDeclaration)
//                    continue;

//                var propertyName = attribute.Name.LocalName;
//                var propertyValue = attribute.Value;

//                if (IsAttachedProperty(attribute.Name.LocalName))
//                {
//                    irElement.AttachedProperties[propertyName] = propertyValue;
//                }
//                else
//                {
//                    irElement.Properties[propertyName] = propertyValue;
//                }
//            }
//            foreach (var child in element.Elements())
//            {
//                var childIr = ConvertElement(child);
//                irElement.Children.Add(childIr);
//            }
//            return irElement;
//        }
//        /// <summary>
//        /// Detects attached properties (e.g., Grid.Row).
//        /// </summary>
//        private bool IsAttachedProperty(string propertyName)
//        {
//            return propertyName.Contains(".");
//        }
//    }
//}