using System;
using System.Collections.Generic;
using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Defines a contract for mapping XAML element types
    /// to corresponding HTML tag names.
    /// </summary>
    public interface IElementTagMapper
    {
        /// <summary>
        /// Returns the HTML tag associated with the given XAML element type.
        /// </summary>
        string Map(string xamlType);
    }
}
