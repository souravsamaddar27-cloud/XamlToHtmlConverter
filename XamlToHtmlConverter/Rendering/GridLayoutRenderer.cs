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
            // Rows
            // Rows
            if (element.GridRowDefinitions.Count > 0)
            {
                var rows = element.GridRowDefinitions
                    .Select(ConvertGridLength);

                sb.Append($"grid-template-rows:{string.Join(" ", rows)};");
            }
            else
            {
                var maxRow = 0;

                foreach (var child in element.Children)
                {
                    var rowIndex = 0;
                    var span = 1;

                    if (child.AttachedProperties.TryGetValue("Grid.Row", out var rowValue) &&
                        int.TryParse(rowValue, out var parsedRow))
                    {
                        rowIndex = parsedRow;
                    }

                    if (child.AttachedProperties.TryGetValue("Grid.RowSpan", out var spanValue) &&
                        int.TryParse(spanValue, out var parsedSpan))
                    {
                        span = parsedSpan;
                    }

                    var lastRow = rowIndex + span - 1;

                    if (lastRow > maxRow)
                        maxRow = lastRow;
                }

                if (element.Children.Count > 0)
                {
                    var rows = new List<string>();
                    for (int i = 0; i <= maxRow; i++)
                        rows.Add("auto");

                    sb.Append($"grid-template-rows:{string.Join(" ", rows)};");
                }
            }

            // Columns
            // Columns
            if (element.GridColumnDefinitions.Count > 0)
            {
                var cols = element.GridColumnDefinitions
                    .Select(ConvertGridLength);

                sb.Append($"grid-template-columns:{string.Join(" ", cols)};");
            }
            else
            {
                var maxCol = 0;

                foreach (var child in element.Children)
                {
                    var colIndex = 0;
                    var span = 1;

                    if (child.AttachedProperties.TryGetValue("Grid.Column", out var colValue) &&
                        int.TryParse(colValue, out var parsedCol))
                    {
                        colIndex = parsedCol;
                    }

                    if (child.AttachedProperties.TryGetValue("Grid.ColumnSpan", out var spanValue) &&
                        int.TryParse(spanValue, out var parsedSpan))
                    {
                        span = parsedSpan;
                    }

                    var lastCol = colIndex + span - 1;

                    if (lastCol > maxCol)
                        maxCol = lastCol;
                }

                if (element.Children.Count > 0)
                {
                    var cols = new List<string>();
                    for (int i = 0; i <= maxCol; i++)
                        cols.Add("auto");

                    sb.Append($"grid-template-columns:{string.Join(" ", cols)};");
                }
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
