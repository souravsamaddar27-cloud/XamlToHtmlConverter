using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    public interface ILayoutRenderer
    {
        bool CanHandle(IrElement element);
        void ApplyLayout(IrElement element, StringBuilder styleBuilder);
    }
}
