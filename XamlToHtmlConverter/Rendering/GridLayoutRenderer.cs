using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    public class GridLayoutRenderer : ILayoutRenderer
    {
        public bool CanHandle(IrElement element)
            => element.Type == "Grid";

        public void ApplyLayout(IrElement element, StringBuilder sb)
        {
            sb.Append("display:grid");
            if (element.GridRowDefinitions.Count > 0)
            {
                var rows = element.GridRowDefinitions
                    .Select(ConvertGridLength);

                sb.Append($"grid-template-rows:{string.Join(" ", rows)};");
            }

            if(element.GridColumnDefinitions.Count > 0)
            {
                var cols = element.GridColumnDefinitions.Select(ConvertGridLength);
                sb.Append($"grid-template-columns:{string.Join(", ", cols)};");
            }

        }
        private string ConvertGridLength(string value)
        {
            value = value.Trim();

            if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase))
                return "auto";

            if (value.EndsWith("*"))
            {
                var numberPart = value.Replace("*", "");
                if (string.IsNullOrWhiteSpace(numberPart))
                    return "1fr";

                if (int.TryParse(numberPart, out var multiplier))
                    return $"{multiplier}fr";
            }
            if (int.TryParse(value, out var pixels))
                return $"{pixels}px";

            return value;
        }
    }
}
