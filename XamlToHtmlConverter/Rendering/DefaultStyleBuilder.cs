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
            if (element.Properties.TryGetValue("Visibility", out var visibility))
            {
                switch (visibility)
                {
                    case "Collapsed":
                        sb.Append("display:none;");
                        return; // Nothing else matters if collapsed
                    case "Hidden":
                        sb.Append("visibility:hidden;");
                        break;
                    case "Visible":
                        // Do nothing
                        break;
                }
            }
            if (element.Properties.TryGetValue("Width", out var width) && int.TryParse(width, out var w))
                sb.Append($"width:{w}px;");

            if (element.Properties.TryGetValue("Height", out var height) && int.TryParse(height, out var h))
                sb.Append($"height:{h}px;");

            // MinWidth
            if (element.Properties.TryGetValue("MinWidth", out var minWidth)
                && int.TryParse(minWidth, out var minW))
            {
                sb.Append($"min-width:{minW}px;");
            }

            // MaxWidth
            if (element.Properties.TryGetValue("MaxWidth", out var maxWidth)
                && int.TryParse(maxWidth, out var maxW))
            {
                sb.Append($"max-width:{maxW}px;");
            }

            // MinHeight
            if (element.Properties.TryGetValue("MinHeight", out var minHeight)
                && int.TryParse(minHeight, out var minH))
            {
                sb.Append($"min-height:{minH}px;");
            }

            // MaxHeight
            if (element.Properties.TryGetValue("MaxHeight", out var maxHeight)
                && int.TryParse(maxHeight, out var maxH))
            {
                sb.Append($"max-height:{maxH}px;");
            }
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
            // Panel.ZIndex
            if (element.AttachedProperties.TryGetValue("Panel.ZIndex", out var zIndex)
                && int.TryParse(zIndex, out var z))
            {
                sb.Append($"z-index:{z};");
            }
        }

        /// <summary>
        /// Applies alignment styles depending on the parent layout type.
        /// Adjusts cross-axis or self-alignment accordingly.
        /// </summary>
        private void ApplyAlignment(IrElement element, LayoutContext context, StringBuilder sb)
        {
            if (string.Equals(context.ParentLayoutType, "Grid", StringComparison.OrdinalIgnoreCase))
            {
                // Horizontal
                if (element.Properties.TryGetValue("HorizontalAlignment", out var hAlign))
                {
                    var css = ConvertAlignment(hAlign);
                    if (css != null)
                        sb.Append($"justify-self:{css};");
                }

                // Vertical
                if (element.Properties.TryGetValue("VerticalAlignment", out var vAlign))
                {
                    var css = ConvertAlignment(vAlign);
                    if (css != null)
                        sb.Append($"align-self:{css};");
                }

                return;
            }

            if (string.Equals(context.ParentLayoutType, "StackPanel", StringComparison.OrdinalIgnoreCase))
            {
                var orientation = context.ParentOrientation ?? "Vertical";

                if (string.Equals(orientation, "Vertical", StringComparison.OrdinalIgnoreCase))
                {
                    // Cross axis = horizontal
                    if (element.Properties.TryGetValue("HorizontalAlignment", out var hAlign))
                    {
                        var css = ConvertAlignment(hAlign);
                        if (css != null)
                            sb.Append($"align-self:{css};");
                    }
                }
                else // Horizontal StackPanel
                {
                    // Cross axis = vertical
                    if (element.Properties.TryGetValue("VerticalAlignment", out var vAlign))
                    {
                        var css = ConvertAlignment(vAlign);
                        if (css != null)
                            sb.Append($"align-self:{css};");
                    }
                }

                return;
            }
        }
        /// <summary>
        /// Converts alignment values from XAML format to CSS equivalents.
        /// </summary>
        private string? ConvertAlignment(string value)
        {
            return value switch
            {
                "Left" => "start",
                "Right" => "end",
                "Top" => "start",
                "Bottom" => "end",
                "Center" => "center",
                "Stretch" => null, // Do not emit
                _ => null
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
        public Dictionary<string, string> ExtractBindingAttributes(IrElement element)
        {
            var result = new Dictionary<string, string>();

            foreach (var prop in element.Properties)
            {
                var value = prop.Value?.Trim();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (value.StartsWith("{Binding") && value.EndsWith("}"))
                {
                    var inner = value
                        .Substring(8, value.Length - 9) // remove "{Binding" and "}"
                        .Trim();

                    string? path = null;

                    // Case 1: Path=Name
                    if (inner.Contains("Path=", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = inner.Split(',');
                        foreach (var part in parts)
                        {
                            var trimmed = part.Trim();
                            if (trimmed.StartsWith("Path=", StringComparison.OrdinalIgnoreCase))
                            {
                                path = trimmed.Substring(5).Trim();
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Case 2: First token is the path
                        var firstPart = inner.Split(',')[0].Trim();
                        path = firstPart;
                    }

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        result[$"data-binding-{prop.Key.ToLower()}"] = path;
                    }
                }
            }

            return result;
        }
    }
}
  