using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    public interface IStyleBuilder
    {
        string Build(IrElement element, LayoutContext context);
    }
}
