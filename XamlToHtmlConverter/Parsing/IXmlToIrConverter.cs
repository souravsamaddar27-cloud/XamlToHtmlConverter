using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Parsing
{
    /// <summary>
    /// Contract for converting XML DOM into IR.
    /// </summary>
    public interface IXmlToIrConverter
    {
        IrElement Convert(XElement element);
    }
}
