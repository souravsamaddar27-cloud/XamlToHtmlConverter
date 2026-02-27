using System;
using System.Collections.Generic;
using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Represents contextual information about the parent layout
    /// used during style generation.
    /// </summary>
    public class LayoutContext
    {
        /// <summary>
        /// Gets the layout type of the parent element
        /// (e.g., Grid, StackPanel).
        /// </summary>
        public string? ParentLayoutType { get; }
        /// <summary>
        /// Initializes the layout context with the parent layout type.
        /// </summary>
        public string? ParentOrientation { get; }
        public LayoutContext(string? parentLayoutType, string? parentOrientation = null)
        {
            ParentLayoutType = parentLayoutType;
            ParentOrientation = parentOrientation;
        }
    }
}
