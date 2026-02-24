using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    public class StackPanelLayoutRenderer : ILayoutRenderer
    {
        public  bool CanHandle(IrElement element) => element.Type == "StackPanel";

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
