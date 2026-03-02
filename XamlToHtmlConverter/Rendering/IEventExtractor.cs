using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Defines a contract for extracting event-related metadata
    /// from IR elements during HTML rendering.
    /// </summary>
    public interface IEventExtractor
    {
        /// <summary>
        /// Extracts event attributes from the specified IR element
        /// and returns them as HTML-compatible key-value pairs.
        /// </summary>
        Dictionary<string, string> Extract(IrElement element);
    }
}
