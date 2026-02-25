using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Default implementation of IStyleBuilder.
    /// Generates inline CSS styles based on standard properties,
    /// attached properties, and layout context.
    /// </summary>
    public class DefaultStyleBuilder : IStyleBuilder
    {
        /// <summary>
        /// Builds the complete CSS style string for the given IR element.
        /// Delegates processing to specialized style handlers.
        /// </summary>
        public string Build(IrElement element, LayoutContext context)
        {
          
            var sb = new StringBuilder();
            ApplyStandardProperties(element, sb);
            ApplyAttachedProperties(element, sb);
            ApplyAlignment(element, context, sb);
            return sb.ToString();
        }
        /// <summary>
        /// Applies width, height, background, margin, and padding styles
        /// based on regular element properties.
        /// </summary>
        private void ApplyStandardProperties(IrElement element, StringBuilder sb)
        {
            if (element.Properties.TryGetValue("Width", out var width) && int.TryParse(width, out var w))
                sb.Append($"Width:{w}px;");

            if (element.Properties.TryGetValue("Height", out var height) && int.TryParse(height, out var h))
                sb.Append($"height:{h}px;");

            if (element.Properties.TryGetValue("Background", out var bg))
                sb.Append($"background-color:{bg};");

            if (element.Properties.TryGetValue("Margin", out var margin))
                sb.Append($"margin:{ConvertThickness(margin)};");

            if (element.Properties.TryGetValue("Padding", out var padding))
                sb.Append($"padding:{ConvertThickness(padding)};");
        }
        /// <summary>
        /// Applies grid-related positioning styles including
        /// row, column, and span handling.
        /// </summary>
        private void ApplyAttachedProperties(IrElement element, StringBuilder sb)
        {
            // ROW
            if (element.AttachedProperties.TryGetValue("Grid.RowSpan", out var rowSpan)
                && int.TryParse(rowSpan, out var rs)
                && element.AttachedProperties.TryGetValue("Grid.Row", out var baseRow)
                && int.TryParse(baseRow, out var r))
            {
                sb.Append($"grid-row:{r + 1} / span {rs};");
            }
            else if (element.AttachedProperties.TryGetValue("Grid.Row", out var row)
                     && int.TryParse(row, out var rr))
            {
                sb.Append($"grid-row:{rr + 1};");
            }

            // COLUMN
            if (element.AttachedProperties.TryGetValue("Grid.ColumnSpan", out var colSpan)
                && int.TryParse(colSpan, out var cs)
                && element.AttachedProperties.TryGetValue("Grid.Column", out var baseCol)
                && int.TryParse(baseCol, out var c))
            {
                sb.Append($"grid-column:{c + 1} / span {cs};");
            }
            else if (element.AttachedProperties.TryGetValue("Grid.Column", out var col)
                     && int.TryParse(col, out var cc))
            {
                sb.Append($"grid-column:{cc + 1};");
            }
        }

        /// <summary>
        /// Applies alignment styles depending on the parent layout type.
        /// Adjusts cross-axis or self-alignment accordingly.
        /// </summary>
        private void ApplyAlignment(IrElement element, LayoutContext context, StringBuilder sb)
        {
            Console.WriteLine($"Alignment check: ParentLayoutType={context.ParentLayoutType}");
            if (string.Equals(context.ParentLayoutType, "Grid", StringComparison.OrdinalIgnoreCase))
            {
                if (element.Properties.TryGetValue("HorizontalAlignment", out var hAlign))
                {
                    sb.Append($"justify-self:{ConvertAlignment(hAlign)};");
                }

                if (element.Properties.TryGetValue("VerticalAlignment", out var vAlign))
                {
                    sb.Append($"align-self:{ConvertAlignment(vAlign)};");
                }
            }
            else if (string.Equals(context.ParentLayoutType, "StackPanel", StringComparison.OrdinalIgnoreCase))
            {
                // In vertical stack (default), horizontal alignment affects cross-axis
                if(element.Properties.TryGetValue("HorizontalAlignment",out var hAlign))
                {
                    sb.Append($"align-self:{ConvertAlignment(hAlign)};");
                }
            }
        }
        /// <summary>
        /// Converts alignment values from XAML format to CSS equivalents.
        /// </summary>
        private string ConvertAlignment(string value)
        {
            return value switch
            {
                "Left" => "start",
                "Right" => "end",
                "Top" => "start",
                "Bottom" => "end",
                "Center" => "center",
                "Stretch" => "stretch",
                _ => "start"
            };
        }
        /// <summary>
        /// Converts XAML Thickness values into CSS spacing format.
        /// </summary>
        private string ConvertThickness(string thickness)
        {
            var parts = thickness.Split(',');
            if (parts.Length == 1)
                return $"{parts[0]}px";
            if (parts.Length == 2)
                return $"{parts[1]}px {parts[0]}px";

            if(parts.Length == 4)
            {
                var left = parts[0];
                var top = parts[1];
                var right= parts[2];
                var bottom = parts[3];

                return $"{top}px {right}px {bottom}px {left}px";
            }
            return thickness;
        }
    }
}
