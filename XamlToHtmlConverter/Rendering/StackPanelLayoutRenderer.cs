using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    public class StackPanelLayoutRenderer : ILayoutRenderer
    {
        public  bool CanHandle(IrElement element) => element.Type == "StackPanel";

        public void ApplyLayout(IrElement element, StringBuilder sb)
        {
            sb.Append("display:flex;flex-direction:column;");
        }
    }
}
