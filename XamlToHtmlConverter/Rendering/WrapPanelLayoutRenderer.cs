using System;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Layout renderer responsible for handling WrapPanel elements.
    /// Converts WrapPanel behavior into a flexbox layout with wrapping.
    /// </summary>
    public class WrapPanelLayoutRenderer : ILayoutRenderer
    {
        /// <summary>
        /// Determines whether this renderer can handle the specified IR element.
        /// </summary>
        public bool CanHandle(IrElement element)
            => element.Type == "WrapPanel";

        /// <summary>
        /// Applies flexbox layout rules for WrapPanel.
        /// Enables wrapping and sets direction based on Orientation property.
        /// Defaults to horizontal (row) layout.
        /// </summary>
        public void ApplyLayout(IrElement element, StringBuilder sb)
        {
            sb.Append("display:flex;");
            sb.Append("flex-wrap:wrap;");

            // Default orientation = Horizontal
            var orientation = "Horizontal";

            if (element.Properties.TryGetValue("Orientation", out var o))
                orientation = o;

            if (string.Equals(orientation, "Vertical", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("flex-direction:column;");
            }
            else
            {
                sb.Append("flex-direction:row;");
            }
        }
    }
}