using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Defines a contract for building CSS style strings
    /// based on element properties and layout context.
    /// </summary>
    public interface IStyleBuilder
    {
        /// <summary>
        /// Generates a CSS style string for the specified IR element,
        /// considering its parent layout context.
        /// </summary>
        string Build(IrElement element, LayoutContext context);
        Dictionary<string, string> ExtractBindingAttributes(IrElement element);
    }
}
