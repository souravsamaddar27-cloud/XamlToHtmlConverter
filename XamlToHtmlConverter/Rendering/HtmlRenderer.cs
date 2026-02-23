using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Responsible for rendering IR elements into a complete HTML document.
    /// Handles tag mapping, layout conversion, and style generation.
    /// </summary>
    public class HtmlRenderer
    {
        /// <summary>
        /// Generates a full HTML document from the root IR element.
        /// Wraps rendered content with standard HTML structure.
        /// </summary>
        public string RenderDocument(IrElement root)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\" />");
            sb.AppendLine("<title>XAML to HTML Output</title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            RenderElement(root, sb, 0);

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();

        }

        private readonly IElementTagMapper _tagMapper;
        private readonly IEnumerable<ILayoutRenderer> _layoutRenderers;
        public HtmlRenderer(IElementTagMapper tagMapper, IEnumerable<ILayoutRenderer> layoutRenderers)
        {
            _tagMapper = tagMapper; 
            _layoutRenderers = layoutRenderers;
        }


        /// <summary>
        /// Recursively renders an IR element and its children
        /// into corresponding HTML markup with indentation.
        /// </summary>
        private void RenderElement(IrElement element, StringBuilder sb, int indent)
        {
            var indentation = new string(' ', indent);
            var tag = _tagMapper.Map(element.Type);
            var style = BuildStyle(element);
            sb.Append($"{indentation}<{tag}");

            if (!string.IsNullOrWhiteSpace(style))
                sb.Append($" style=\"{style}\"");
            sb.Append(">");

            if (!string.IsNullOrWhiteSpace(element.InnerText))
                sb.Append(element.InnerText);

            if (element.Children.Count > 0)
                sb.AppendLine();

            foreach (var child in element.Children)
            {
                RenderElement(child, sb, indent + 2);
            }
            if (element.Children.Count > 0)
                sb.Append(indentation);

            sb.Append($"</{tag}>");

        }

        /// <summary>
        /// Converts XAML Thickness values into CSS-compatible spacing format.
        /// Supports single, double, and four-value formats.
        /// </summary>
        private string ConvertThicknessToCss(string thickness)
        {
            var parts = thickness.Split(',');
            if (parts.Length == 1)
                return $"{parts[0]}px";

            if (parts.Length == 2)
                return $"{parts[1]}px {parts[0]}px";

            if (parts.Length == 4)
            {
                var left = parts[0];
                var top = parts[1];
                var right = parts[2];
                var bottom = parts[3];

                return $"{top}px {right}px {bottom}px {left}px";
            }
            return thickness;
        }

       private void ApplyGridTemplate(IrElement element, StringBuilder sb)
        {
            if (element.Type != "Grid")
                return;

            if (element.GridRowDefinitions.Count > 0)
            {
                var rows = element.GridRowDefinitions.Select(ConvertGridLength).ToList();
                sb.Append($"grid-template-rows:{string.Join(" ", rows)};");
            }
            if(element.GridColumnDefinitions.Count > 0)
            {
                var cols = element.GridColumnDefinitions.Select(ConvertGridLength).ToList();
                sb.Append($"grid-template-columns:{string.Join(" ", cols)};");
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
                if(string.IsNullOrWhiteSpace(numberPart)) 
                    return "1fr";

                if (int.TryParse(numberPart, out var multiplier))
                    return $"{multiplier}fr";
            }
            if (int.TryParse(value, out var pixels))
                return $"{pixels}px";

            return value;
        }
       

        /// <summary>
        /// Builds inline CSS styles based on element type,
        /// layout behavior, standard properties, and attached properties.
        /// </summary>
        private string BuildStyle(IrElement element)
        {
            var sb = new StringBuilder();

            //Layout mapping
            if (element.Type == "Grid")
                sb.Append("display:grid;");
                ApplyGridTemplate(element, sb);

            if (element.Type == "StackPanel")
            {
                foreach (var layout in _layoutRenderers)
                {
                    if (layout.CanHandle(element))
                    {
                        layout.ApplyLayout(element, sb);
                        break;
                    }
                }
            }

            //Width / Height
            if (element.Properties.TryGetValue("Width", out var width))
                sb.Append($"width:{width}px;");

            if (element.Properties.TryGetValue("Height", out var height))
                sb.Append($"height:{height}px;");

            //Background
            if (element.Properties.TryGetValue("Background", out var bg))
                sb.Append($"background-color:{bg};");

            //Grid
            if(element.AttachedProperties.TryGetValue("Grid.Row",out var row))
            {
                if (int.TryParse(row, out var r))
                    sb.Append($"grid-row:{r + 1};");
            }
            if(element.AttachedProperties.TryGetValue("Grid.Column", out var col))
            {
                if (int.TryParse(col, out var c))
                {
                    if(element.AttachedProperties.TryGetValue("Grid.ColumnSpan",out var span) && int.TryParse(span, out var s))
                    {
                        sb.Append($"grid-column:{c + 1} / span {s};");
                    }
                    else
                    {
                        sb.Append($"grid-column:{c + 1};");
                    }
                }
            }
            if(element.AttachedProperties.TryGetValue("Grid.RowSpan",out var rowSpan))
            {
                if(int.TryParse(rowSpan, out var rs))
                {
                    if(element.AttachedProperties.TryGetValue("Grid.Row", out var baseRow) && int.TryParse(baseRow, out var r)){
                        sb.Append($"grid-row:{r + 1} / span {rs};");
                    }
                }
            }

            ///CSS Margins
            if(element.Properties.TryGetValue("Margin", out var margin))
            {
                sb.Append($"margin:{ConvertThicknessToCss(margin)};");
            }
            if(element.Properties.TryGetValue("Padding", out var padding))
            {
                sb.Append($"padding:{ConvertThicknessToCss(padding)};");
            }

            return sb.ToString();
        }
    }
}
