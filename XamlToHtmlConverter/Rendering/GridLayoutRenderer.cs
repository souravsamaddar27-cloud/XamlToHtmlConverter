using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Layout renderer responsible for handling Grid elements.
    /// Applies CSS Grid layout rules and template definitions.
    /// </summary>
    public class GridLayoutRenderer : ILayoutRenderer
    {
        /// <summary>
        /// Determines whether this renderer can process the given element.
        /// </summary>
        public bool CanHandle(IrElement element)
            => element.Type == "Grid";

        /// <summary>
        /// Applies CSS grid layout styles including
        /// row and column template definitions.
        /// </summary>
        public void ApplyLayout(IrElement element, StringBuilder sb)
        {
            sb.Append("display:grid;");
            if (element.GridRowDefinitions.Count > 0)
            {
                var rows = element.GridRowDefinitions
                    .Select(ConvertGridLength);

                sb.Append($"grid-template-rows:{string.Join(" ", rows)};");
            }

            if(element.GridColumnDefinitions.Count > 0)
            {
                var cols = element.GridColumnDefinitions.Select(ConvertGridLength);
                sb.Append($"grid-template-columns:{string.Join(" ", cols)};");
            }

        }
        /// <summary>
        /// Converts XAML GridLength values into
        /// corresponding CSS units (auto, fr, px).
        /// </summary>
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
