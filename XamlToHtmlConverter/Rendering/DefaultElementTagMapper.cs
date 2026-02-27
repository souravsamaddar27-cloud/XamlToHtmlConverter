using System;
using System.Collections.Generic;
using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Maps IR element types to corresponding HTML tags.
    /// Defaults to 'div' when no specific mapping exists.
    /// </summary>
    public class DefaultElementTagMapper : IElementTagMapper
    {
        private readonly Dictionary<string, string> _map =new Dictionary<string, string>
            {
                {"Grid","div"},
                {"StackPanel","div"},
                {"Button","button"},
                {"TextBlock","span"},
                {"Border","div"},
                {"CheckBox","input"},
                {"RadioButton","input"},
                {"Image","img"},
                {"ComboBox","select"},
                {"ListBox","select"},
                {"ComboBoxItem","option"},
                {"ListBoxItem","option"},
                {"ContentControl","div"},
                {"TextBox","input"}
            };
        public string Map(string xamlType)
        {
            if(_map.TryGetValue(xamlType, out var tag))
                return tag;
            return "div"; //fallback
        }
    }
}
