using System;
using System.Collections.Generic;
using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    public class LayoutContext
    {
        public string? ParentLayoutType { get; }
        public LayoutContext(string? parentLayoutType)
        {
            ParentLayoutType = parentLayoutType;
        }
    }
}
