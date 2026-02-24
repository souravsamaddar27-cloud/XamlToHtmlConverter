using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Defines a contract for applying layout-specific
    /// styling behavior to IR elements.
    /// </summary>
    public interface ILayoutRenderer
    {
        /// <summary>
        /// Determines whether the renderer can handle
        /// the specified IR element type.
        /// </summary>
        bool CanHandle(IrElement element);
        /// <summary>
        /// Applies layout-related CSS styles
        /// to the provided style builder.
        /// </summary>
        void ApplyLayout(IrElement element, StringBuilder styleBuilder);
    }
}
