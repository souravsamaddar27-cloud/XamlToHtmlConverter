using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Layout renderer responsible for handling StackPanel elements.
    /// Converts StackPanel behavior into corresponding flexbox layout styles.
    /// </summary>
    public class StackPanelLayoutRenderer : ILayoutRenderer
    {
        /// <summary>
        /// Determines whether this renderer can handle the specified IR element.
        /// </summary>
        public bool CanHandle(IrElement element) => element.Type == "StackPanel";

        /// <summary>
        /// Applies flexbox layout styles based on StackPanel orientation.
        /// Defaults to vertical (column) direction.
        /// </summary>
        public void ApplyLayout(IrElement element, StringBuilder sb)
        {
            sb.Append("display:flex;");
            //Default is Vertical
            var direction = "column";
            if(element.Properties.TryGetValue("Orientation", out var orientation))
            {
                if (orientation.Equals("Horizontal", StringComparison.OrdinalIgnoreCase))
                    direction = "row";
            }
            sb.Append($"flex-direction:{direction};");
        }
    }
}
