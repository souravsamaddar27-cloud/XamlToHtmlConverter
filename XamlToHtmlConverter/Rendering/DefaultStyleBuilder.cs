// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System;
using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.StyleMappers;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Default implementation of <see cref="IStyleBuilder"/>.
    /// Generates inline CSS styles from standard element properties,
    /// attached layout properties, and parent layout context.
    /// 
    /// Satisfies Dependency Inversion Principle:
    ///   - Property mappers are injected through constructor
    ///   - Allows substitution and testing of mapper implementations
    ///   - Factory provides default set of mappers
    /// </summary>
    public class DefaultStyleBuilder : IStyleBuilder
    {
        private readonly PropertyMapperEngine v_PropertyEngine;
        private readonly Dictionary<string, string> v_StyleRegistry = new();
        private int v_StyleCounter = 0;

        /// <summary>
        /// Initializes with the specified property mappers.
        /// Enables dependency injection and extension (D and O principles).
        /// </summary>
        /// <param name="mappers">The collection of property mappers to use.</param>
        public DefaultStyleBuilder(IEnumerable<IPropertyMapper> mappers)
        {
            v_PropertyEngine = new PropertyMapperEngine(mappers);
        }

        #region Public Methods

        /// <summary>
        /// Registers a CSS style string and returns a reusable class name.
        /// </summary>
        /// <param name="cssStyle">The CSS style string to register.</param>
        /// <returns>A class name that can be applied to HTML elements.</returns>
        public string Register(string cssStyle)
        {
            if (string.IsNullOrWhiteSpace(cssStyle))
                return string.Empty;

            var className = $"style-{v_StyleCounter++}";
            v_StyleRegistry[className] = cssStyle;
            return className;
        }

        /// <summary>
        /// Generates the complete &lt;style&gt; block for the HTML &lt;head&gt;.
        /// </summary>
        /// <returns>The consolidated CSS style block.</returns>
        public string GenerateStyleBlock()
        {
            if (v_StyleRegistry.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("<style>");

            foreach (var kvp in v_StyleRegistry)
            {
                sb.AppendLine($"  .{kvp.Key} {{ {kvp.Value} }}");
            }

            sb.AppendLine("</style>");
            return sb.ToString();
        }

        /// <summary>
        /// Builds the complete inline CSS style string for the given IR element
        /// by delegating to specialized style handlers for standard properties,
        /// attached properties, and alignment.
        /// </summary>
        /// <param name="element">The IR element to generate CSS styles for.</param>
        /// <param name="context">The layout context describing the parent container type and orientation.</param>
        /// <returns>A combined CSS style string ready for use as an HTML style or class attribute value.</returns>
        public string Build(IntermediateRepresentationElement element, LayoutContext context)
        {
            var sb = new StringBuilder();
            v_PropertyEngine.Apply(element, context, sb);
            ApplyAttachedProperties(element, context, sb);
            //ApplyAlignment(element, context, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Extracts data binding expressions from the IR element's properties
        /// and returns them as HTML data-binding-* attribute key-value pairs.
        /// </summary>
        /// <param name="element">The IR element to scan for binding expressions.</param>
        /// <returns>
        /// A dictionary mapping HTML attribute names (e.g., data-binding-text)
        /// to the corresponding binding path strings.
        /// </returns>
        public Dictionary<string, string> ExtractBindingAttributes(IntermediateRepresentationElement element)
        {
            var result = new Dictionary<string, string>();

            foreach (var binding in element.Bindings)
            {
                var path = binding.Value.Path;

                if (!string.IsNullOrWhiteSpace(path))
                {
                                var key = binding.Key;

                                if (key == "IsSelected")
                                    key = "selected";
                                else if (key == "IsChecked")
                                    key = "checked";
                                else if (key == "IsEnabled")
                                    key = "enabled";
                                else if (key == "Visibility")
                                    key = "visibility";
                                else
                                    key = key.ToLower();

                                result[$"data-binding-{key}"] = path;
                }
            }
            foreach (var trigger in element.Triggers)
            {
                var key = $"data-trigger-{trigger.Property.ToLower()}";

                var sb_setters = new StringBuilder();
                bool first = true;
                foreach (var s in trigger.Setters)
                {
                    if (!first) sb_setters.Append(";");
                    sb_setters.Append(s.Key).Append(":").Append(s.Value);
                    first = false;
                }

                result[key] = $"{trigger.Value}:{sb_setters}";
            }

            var multi = TriggerEngine.ExtractMultiTriggers(element);

            foreach (var m in multi)
                result[m.Key] = m.Value;

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Appends width, height, min/max dimensions, background color, margin, and padding
        /// CSS properties derived from the element's standard XAML properties.
        /// Returns early with display:none when Visibility is Collapsed.
        /// </summary>
        /// <param name="element">The IR element to read properties from.</param>
        /// <param name="sb">The string builder to append CSS to.</param>
        private void ApplyStandardProperties(
    IntermediateRepresentationElement element,
    LayoutContext context,
    StringBuilder sb)
        {
            if (element.Properties.TryGetValue("Visibility", out var visibility))
            {
                switch (visibility)
                {
                    case "Collapsed":
                        sb.Append("display:none;");
                        return;
                    case "Hidden":
                        sb.Append("visibility:hidden;");
                        break;
                    case "Visible":
                        break;
                }
            }

            if (element.Properties.TryGetValue("Width", out var width))
            {
                var widthLower = width.ToLowerInvariant();
                if (widthLower == "auto")
                {
                    sb.Append("width:auto;");
                }
                else if (widthLower == "stretch")
                {
                    sb.Append("width:100%;");
                }
                else if (int.TryParse(width, out var w))
                {
                    // Responsive improvement
                    sb.Append($"max-width:{w}px;");
                    sb.Append("width:100%;");
                }
            }

            if (element.Properties.TryGetValue("Height", out var height))
            {
                var heightLower = height.ToLowerInvariant();
                if (heightLower == "auto")
                {
                    sb.Append("height:auto;");
                }
                else if (heightLower == "stretch")
                {
                    sb.Append("height:100%;");
                }
                else if (int.TryParse(height, out var h))
                {
                    // Better vertical behavior
                    sb.Append($"min-height:{h}px;");
                }
            }

            if (element.Properties.TryGetValue("MinWidth", out var minWidth) && int.TryParse(minWidth, out var minW))
                sb.Append($"min-width:{minW}px;");

            if (element.Properties.TryGetValue("MaxWidth", out var maxWidth) && int.TryParse(maxWidth, out var maxW))
                sb.Append($"max-width:{maxW}px;");

            if (element.Properties.TryGetValue("MinHeight", out var minHeight) && int.TryParse(minHeight, out var minH))
                sb.Append($"min-height:{minH}px;");

            if (element.Properties.TryGetValue("MaxHeight", out var maxHeight) && int.TryParse(maxHeight, out var maxH))
                sb.Append($"max-height:{maxH}px;");

            if (element.Properties.TryGetValue("Background", out var bg))
                sb.Append($"background-color:{bg};");

            if (element.Properties.TryGetValue("Margin", out var margin))
                sb.Append($"margin:{ConvertThickness(margin)};");

            if (element.Properties.TryGetValue("Padding", out var padding))
                sb.Append($"padding:{ConvertThickness(padding)};");

            // Default spacing between controls when XAML did not specify margin
            if (!element.Properties.ContainsKey("Margin"))
            {
                // Do not add margin to layout containers
                if (element.Type != "Grid" &&
                    element.Type != "StackPanel" &&
                    element.Type != "DockPanel" &&
                    element.Type != "WrapPanel")
                {
                    sb.Append("margin:4px;");
                }
            }

            // ---- Typography Mapping (WPF ? CSS) ----

            // FontSize
            if (element.Properties.TryGetValue("FontSize", out var fontSize) &&
                int.TryParse(fontSize, out var fs))
            {
                sb.Append($"font-size:{fs}px;");
            }

            // FontWeight
            if (element.Properties.TryGetValue("FontWeight", out var fontWeight))
            {
                sb.Append($"font-weight:{fontWeight.ToLower()};");
            }

            // FontFamily
            if (element.Properties.TryGetValue("FontFamily", out var fontFamily))
            {
                sb.Append($"font-family:{fontFamily};");
            }

            // Foreground color
            if (element.Properties.TryGetValue("Foreground", out var foreground))
            {
                sb.Append($"color:{foreground};");
            }

            // WrapPanel item sizing support
            if (context.ParentLayoutType == "WrapPanel")
            {
                sb.Append("width:var(--wrap-item-width,auto);");
                sb.Append("height:var(--wrap-item-height,auto);");
            }
        }

        /// <summary>
        /// Appends grid row, column, span, and z-index CSS positioning properties
        /// derived from the element's attached XAML properties.
        /// </summary>
        /// <param name="element">The IR element to read attached properties from.</param>
        /// <param name="sb">The string builder to append CSS to.</param>
        private void ApplyAttachedProperties(IntermediateRepresentationElement element, LayoutContext context, StringBuilder sb)
        {
            // Grid row positioning
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

            // Grid column positioning
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

            // Z-index
            if (element.AttachedProperties.TryGetValue("Panel.ZIndex", out var zIndex)
                && int.TryParse(zIndex, out var z))
            {
                sb.Append($"z-index:{z};");
            }

            // DockPanel positioning
            if (element.AttachedProperties.TryGetValue("DockPanel.Dock", out var dock))
            {
                switch (dock)
                {
                    case "Top":
                        sb.Append("align-self:stretch;");
                        sb.Append("width:100%;");
                        sb.Append("order:-1;");
                        break;

                    case "Bottom":
                        sb.Append("align-self:stretch;");
                        sb.Append("width:100%;");
                        sb.Append("order:2;");
                        break;

                    case "Left":
                        sb.Append("display:flex;");
                        sb.Append("flex-direction:column;");
                        sb.Append("flex:0 0 auto;");
                        break;

                    case "Right":
                        sb.Append("display:flex;");
                        sb.Append("flex-direction:column;");
                        sb.Append("flex:0 0 auto;");
                        sb.Append("margin-left:auto;");
                        break;
                }
            }
            // LastChildFill behavior
            if (context.ParentLayoutType == "DockPanel")
            {
                if (!element.AttachedProperties.ContainsKey("DockPanel.Dock"))
                {
                    sb.Append("flex:1;");
                    
                }
            }
        }

        /// <summary>
        /// Appends alignment CSS properties (justify-self, align-self) to the style builder
        /// based on the element's HorizontalAlignment and VerticalAlignment properties,
        /// adjusted according to the parent layout type and orientation.
        /// </summary>
        /// <param name="element">The IR element whose alignment properties are evaluated.</param>
        /// <param name="context">The layout context of the parent container.</param>
        /// <param name="sb">The string builder to append CSS to.</param>
    //    private void ApplyAlignment(
    //IntermediateRepresentationElement element,
    //LayoutContext context,
    //StringBuilder styleBuilder)
    //    {
    //        if (context.ParentLayoutType == null)
    //            return;

    //        element.Properties.TryGetValue("HorizontalAlignment", out var hAlign);
    //        element.Properties.TryGetValue("VerticalAlignment", out var vAlign);

    //        // GRID ALIGNMENT
    //        if (context.ParentLayoutType == "Grid")
    //        {
    //            var h = ConvertAlignment(hAlign);
    //            var v = ConvertAlignment(vAlign);

    //            styleBuilder.Append($"justify-self:{h};");
    //            styleBuilder.Append($"align-self:{v};");

    //            // WPF Stretch support
    //            if (h == "stretch")
    //                styleBuilder.Append("width:100%;");

    //            if (v == "stretch")
    //                styleBuilder.Append("height:100%;");

    //            return;
    //        }

    //        // STACKPANEL
    //        if (context.ParentLayoutType == "StackPanel")
    //        {
    //            var orientation = context.ParentOrientation ?? "Vertical";

    //            if (orientation == "Vertical")
    //            {
    //                // Cross axis = horizontal
    //                if (!string.IsNullOrWhiteSpace(hAlign))
    //                {
    //                    var h = ConvertAlignment(hAlign);

    //                    styleBuilder.Append($"align-self:{h};");

    //                    if (h == "stretch")
    //                        styleBuilder.Append("width:100%;");
    //                }
    //            }
    //            else
    //            {
    //                // Cross axis = vertical
    //                if (!string.IsNullOrWhiteSpace(vAlign))
    //                {
    //                    var v = ConvertAlignment(vAlign);

    //                    styleBuilder.Append($"align-self:{v};");

    //                    if (v == "stretch")
    //                        styleBuilder.Append("height:100%;");
    //                }
    //            }

    //            return;
    //        }

    //        // WRAPPANEL
    //        if (context.ParentLayoutType == "WrapPanel")
    //        {
    //            if (!string.IsNullOrWhiteSpace(hAlign))
    //            {
    //                var h = ConvertAlignment(hAlign);
    //                styleBuilder.Append($"align-self:{h};");

    //                if (h == "stretch")
    //                    styleBuilder.Append("width:100%;");
    //            }

    //            if (!string.IsNullOrWhiteSpace(vAlign))
    //            {
    //                var v = ConvertAlignment(vAlign);
    //                styleBuilder.Append($"align-self:{v};");

    //                if (v == "stretch")
    //                    styleBuilder.Append("height:100%;");
    //            }
    //        }
    //    }

        /// <summary>
        /// Converts a XAML alignment value into its equivalent CSS flexbox/grid self-alignment keyword.
        /// Returns <c>null</c> for Stretch since that is the default and requires no explicit CSS.
        /// </summary>
        /// <param name="value">The XAML alignment value (e.g., Left, Right, Center, Stretch).</param>
        /// <returns>The CSS keyword (e.g., start, end, center), or <c>null</c> for Stretch or unrecognized values.</returns>
        private string ConvertAlignment(string? alignment)
        {
            if (string.IsNullOrWhiteSpace(alignment))
                return "stretch";

            return alignment switch
            {
                "Left" => "start",
                "Top" => "start",
                "Right" => "end",
                "Bottom" => "end",
                "Center" => "center",
                "Stretch" => "stretch",
                _ => "stretch"
            };
        }

        /// <summary>
        /// Converts a XAML Thickness value into a CSS spacing shorthand string.
        /// Supports single, two-value (left,top), and four-value (left,top,right,bottom) formats.
        /// </summary>
        /// <param name="thickness">The raw XAML Thickness string (e.g., "10", "10,5", "10,5,10,5").</param>
        /// <returns>The equivalent CSS margin or padding value string (e.g., "10px", "5px 10px").</returns>
        private string ConvertThickness(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "0";

            var parts = value.Split(',');

            if (parts.Length == 1)
            {
                if (int.TryParse(parts[0], out var all))
                    return $"{all}px";
            }

            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out var vertical) &&
                    int.TryParse(parts[1], out var horizontal))
                {
                    return $"{vertical}px {horizontal}px";
                }
            }

            if (parts.Length == 4)
            {
                var values = new List<string>();

                foreach (var p in parts)
                {
                    if (int.TryParse(p, out var px))
                        values.Add($"{px}px");
                }

                if (values.Count == 4)
                {
                    var sb_thickness = new StringBuilder();
                    for (int i = 0; i < values.Count; i++)
                    {
                        if (i > 0) sb_thickness.Append(" ");
                        sb_thickness.Append(values[i]);
                    }
                    return sb_thickness.ToString();
                }
            }

            return value;
        }

        #endregion
    }
}
