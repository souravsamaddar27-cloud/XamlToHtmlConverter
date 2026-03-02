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
            var bodyBuilder = new StringBuilder();
            RenderElement(root, bodyBuilder, 0, null, null);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\" />");
            sb.AppendLine("<title>XAML to HTML Output</title>");
            sb.AppendLine(_styleRegistry.GenerateStyleBlock());
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.Append(bodyBuilder.ToString());
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

        private readonly IEventExtractor _eventExtractor;

        /// <summary>
        /// Initializes renderer with required mapping, layout, and styling services.
        /// </summary>
        public HtmlRenderer(IElementTagMapper tagMapper, IEnumerable<ILayoutRenderer> layoutRenderers, IStyleBuilder styleBuilder, IEventExtractor eventExtractor)
        {
            _tagMapper = tagMapper; 
            _layoutRenderers = layoutRenderers;
             _styleBuilder= styleBuilder;
            _eventExtractor = eventExtractor;
        }

        private readonly StyleRegistry _styleRegistry = new();

        /// <summary>
        /// Recursively renders an IR element and its children
        /// into corresponding HTML markup with indentation.
        /// </summary>
        private void RenderElement(IrElement element, StringBuilder sb, int indent, string? parentLayoutType,string? parentOrientation)
        {
            var indentation = new string(' ', indent);
            var tag = _tagMapper.Map(element.Type);
            var style = BuildStyle(element, parentLayoutType, parentOrientation);

            sb.Append($"{indentation}<{tag}");
            if (element.Type == "ListBox")
            {
                sb.Append(" multiple");
            }
            // ---- Binding metadata ----
            var bindingAttributes = _styleBuilder.ExtractBindingAttributes(element);

            foreach (var attr in bindingAttributes)
            {
                sb.Append($" {attr.Key}=\"{attr.Value}\"");
            }

            var eventAttributes = _eventExtractor.Extract(element);
            foreach (var evt in eventAttributes)
            {
                sb.Append($" {evt.Key}=\"{evt.Value}\"");
            }
            // ---- TextBox special handling ----
            if (element.Type == "TextBox")
            {
                sb.Append(" type=\"text\"");

                if (element.Properties.TryGetValue("Text", out var text))
                {
                    var trimmed = text.Trim();

                    bool isBinding =
                        trimmed.StartsWith("{Binding") &&
                        trimmed.EndsWith("}");

                    if (!isBinding)
                    {
                        sb.Append($" value=\"{text}\"");
                    }
                }
            }
            // CheckBox handling
            else if (element.Type == "CheckBox")
            {
                sb.Append(" type=\"checkbox\"");

                if (element.Properties.TryGetValue("IsChecked", out var isChecked) &&
                    string.Equals(isChecked, "True", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" checked");
                }
            }
            // RadioButton handling
            else if (element.Type == "RadioButton")
            {
                sb.Append(" type=\"radio\"");

                if (element.Properties.TryGetValue("IsChecked", out var isChecked) &&
                    string.Equals(isChecked, "True", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" checked");
                }
            }

            // Apply CSS class (deduplicated style)
            if (!string.IsNullOrWhiteSpace(style))
            {
                var className = _styleRegistry.Register(style);
                sb.Append($" class=\"{className}\"");
            }

            // Self-closing elements (input, img)
            if (tag == "input" || tag == "img")
            {
                if (element.Type == "Image" &&
                    element.Properties.TryGetValue("Source", out var src))
                {
                    sb.Append($" src=\"{src}\"");
                }

                sb.Append(" />");

                // Render content for CheckBox or RadioButton
                if ((element.Type == "CheckBox" || element.Type == "RadioButton") &&
                    element.Properties.TryGetValue("Content", out var content))
                {
                    sb.Append($" {content}");
                }

                sb.AppendLine();
                return;
            }

            sb.Append(">");

            // Inner text or content mapping
            if (!string.IsNullOrWhiteSpace(element.InnerText))
            {
                sb.Append(element.InnerText);
            }
            else if ((element.Type == "ContentControl" || element.Type == "Button") &&
                     element.Properties.TryGetValue("Content", out var content))
            {
                sb.Append(content);
            }
            // Render children recursively
            if (element.Children.Count > 0)
            {
                sb.AppendLine();
                foreach (var child in element.Children)
                {
                    string? orientation = null;

                    if (element.Type == "StackPanel" &&
                        element.Properties.TryGetValue("Orientation", out var o))
                    {
                        orientation = o;
                    }

                    // 🔹 Flatten ItemsControl.Items
                    if (child.Type == "ItemsControl.Items")
                    {
                        foreach (var item in child.Children)
                        {
                            RenderElement(item, sb, indent + 2, element.Type, orientation);
                        }
                    }

                    // 🔹 Handle ItemTemplate
                    else if (child.Type == "ItemsControl.ItemTemplate")
                    {
                        foreach (var templateNode in child.Children)
                        {
                            if (templateNode.Type == "DataTemplate")
                            {
                                sb.AppendLine($"{new string(' ', indent + 2)}<template>");

                                foreach (var templateChild in templateNode.Children)
                                {
                                    RenderElement(templateChild, sb, indent + 4, element.Type, orientation);
                                }

                                sb.AppendLine($"{new string(' ', indent + 2)}</template>");
                            }
                        }
                    }

                    else
                    {
                        RenderElement(child, sb, indent + 2, element.Type, orientation);
                    }
                }

                sb.Append(indentation);
            }

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
        private string BuildStyle(IrElement element, string? parentLayoutType, string? parentOrientation)
        {
            var sb = new StringBuilder();

            // 1️⃣ Apply layout container behavior (Grid, StackPanel, etc.)
            // Layout renderers (Grid, StackPanel, DockPanel, etc.)
            foreach (var layout in _layoutRenderers)
            {
                if (layout.CanHandle(element))
                {
                    layout.ApplyLayout(element, sb);
                    break;
                }
            }

            // 2️⃣ Apply property-based styling (width, margin, alignment, grid positioning, etc.)
            var context = new LayoutContext(parentLayoutType, parentOrientation);
            sb.Append(_styleBuilder.Build(element, context));

            return sb.ToString();
        }

    }
}
