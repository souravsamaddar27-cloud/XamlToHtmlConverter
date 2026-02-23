using System;
using System.Collections.Generic;
using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    public interface IElementTagMapper
    {
        string Map(string xamlType);
    }
}
