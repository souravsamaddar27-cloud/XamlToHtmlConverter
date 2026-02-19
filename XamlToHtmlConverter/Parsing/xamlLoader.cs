using System;
using System.IO;
using System.Xml.Linq;

namespace XamlToHtmlConverter.Parsing
{
    /// <summary>
    /// Responsible only for loading XAML into an XML DOM.
    /// No IR logic here.
    /// </summary>
    public class XamlLoader
    {
        public XDocument Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path cannot be null or empty");

            if (!File.Exists(path))
                throw new FileNotFoundException("XAML file not found at ", path);

            return XDocument.Load(path);
        }
    }
}