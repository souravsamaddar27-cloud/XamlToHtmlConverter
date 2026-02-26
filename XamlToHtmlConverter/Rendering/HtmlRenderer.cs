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

            RenderElement(root, sb, 0,null);

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();

        }

        /// <summary>
        /// Maps IR element types to HTML tags.
        /// </summary>
        private readonly IElementTagMapper _tagMapper;
        /// <summary>
        /// Collection of layout renderers responsible for container layout behavior.
        /// </summary>
        private readonly IEnumerable<ILayoutRenderer> _layoutRenderers;
        /// <summary>
        /// Builds inline CSS styles for elements.
        /// </summary>
        private readonly IStyleBuilder _styleBuilder;

        /// <summary>
        /// Initializes renderer with required mapping, layout, and styling services.
        /// </summary>
        public HtmlRenderer(IElementTagMapper tagMapper, IEnumerable<ILayoutRenderer> layoutRenderers, IStyleBuilder styleBuilder)
        {
            _tagMapper = tagMapper; 
            _layoutRenderers = layoutRenderers;
             _styleBuilder= styleBuilder;
        }


        /// <summary>
        /// Recursively renders an IR element and its children
        /// into corresponding HTML markup with indentation.
        /// </summary>
        private void RenderElement( IrElement element,StringBuilder sb, int indent,string? parentLayoutType)
        {
            var indentation = new string(' ', indent);
            var tag = _tagMapper.Map(element.Type);
            var style = BuildStyle(element, parentLayoutType);

            sb.Append($"{indentation}<{tag}");

            // 1️⃣ Apply style
            if (!string.IsNullOrWhiteSpace(style))
            {
                sb.Append($" style=\"{style}\"");
            }

            // 2️⃣ Apply binding attributes
            var bindingAttributes = _styleBuilder.ExtractBindingAttributes(element);
            foreach (var attr in bindingAttributes)
            {
                sb.Append($" {attr.Key}=\"{attr.Value}\"");
            }

            sb.Append(">");

            if (!string.IsNullOrWhiteSpace(element.InnerText))
                sb.Append(element.InnerText);

            if (element.Children.Count > 0)
                sb.AppendLine();

            foreach (var child in element.Children)
            {
                // THIS MUST USE element.Type
                RenderElement(child, sb, indent + 2, element.Type);
            }

            if (element.Children.Count > 0)
                sb.Append(indentation);

            sb.AppendLine($"</{tag}>");
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

        /// <summary>
        /// Applies grid template row and column definitions when element is a Grid.
        /// </summary>
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

        /// <summary>
        /// Converts XAML GridLength values into CSS units (auto, fr, px).
        /// </summary>
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
        private string BuildStyle(IrElement element, string? parentLayoutType)
        {
            var sb = new StringBuilder();

            // 1️⃣ Apply layout container behavior (Grid, StackPanel, etc.)
            foreach (var layout in _layoutRenderers)
            {
                if (layout.CanHandle(element))
                {
                    layout.ApplyLayout(element, sb);
                    break;
                }
            }

            // 2️⃣ Apply property-based styling (width, margin, alignment, grid positioning, etc.)
            var context = new LayoutContext(parentLayoutType);
            sb.Append(_styleBuilder.Build(element, context));

            return sb.ToString();
        }

    }
}
