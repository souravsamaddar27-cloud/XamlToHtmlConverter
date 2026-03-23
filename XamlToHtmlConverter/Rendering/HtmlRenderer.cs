// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Collections.Concurrent;
using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.Behavior;
using XamlToHtmlConverter.Rendering.ControlRenderers;
using XamlToHtmlConverter.Rendering.Templates;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Responsible for rendering an IR element tree into a complete HTML document.
    /// Orchestrates tag mapping, layout CSS generation, style deduplication,
    /// binding attribute emission, and event attribute emission.
    /// </summary>
    public class HtmlRenderer
    {
        

        
        #region Private Data

        /// <summary>
        /// Holds the mapper used to convert XAML element type names to HTML tag names.
        /// </summary>
        private readonly IElementTagMapper v_TagMapper;

        /// <summary>
        /// Holds the collection of layout renderers responsible for container layout behavior.
        /// </summary>
        private readonly IEnumerable<ILayoutRenderer> v_LayoutRenderers;

        /// <summary>
        /// Holds the style builder used to generate inline CSS styles for elements.
        /// </summary>
        private readonly IStyleBuilder v_StyleBuilder;

        /// <summary>
        /// Holds the event extractor used to retrieve event handler attributes from elements.
        /// </summary>
        private readonly IEventExtractor v_EventExtractor;

        /// <summary>
        /// Holds the style registry used to deduplicate inline CSS styles into reusable classes.
        /// </summary>
        private readonly IStyleRegistry v_StyleRegistry;

        /// <summary>
        /// Thread-safe cache for layout renderer resolution by element type.
        /// Maps element type name to resolved layout renderer (eliminates LINQ Where/OrderByDescending).
        /// Uses ConcurrentDictionary to safely handle multi-threaded rendering scenarios.
        /// Performance optimization: ~90% hit rate for typical XAML documents.
        /// </summary>
        private readonly ConcurrentDictionary<string, ILayoutRenderer?> v_LayoutRendererCache = new();

        /// <summary>
        /// Thread-safe cache for HTML tag mapping by element type.
        /// Maps XAML element type to HTML tag name (div, button, span, etc.).
        /// Uses ConcurrentDictionary to safely handle multi-threaded rendering scenarios.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> v_TagMappingCache = new();

        private readonly ControlRendererRegistry v_ControlRegistry;

        private readonly ITemplateEngine v_TemplateEngine;

        private readonly BehaviorRegistry v_BehaviorRegistry;
        #endregion

        private static string GetIndent(int indent)
        => IndentCache.Get(indent);

        #region Constructors

        /// <summary>
        /// Initializes the renderer with required mapping, layout, styling, and event services.
        /// </summary>
        /// <param name="tagMapper">The mapper for converting XAML types to HTML tags.</param>
        /// <param name="layoutRenderers">The collection of layout-specific CSS renderers.</param>
        /// <param name="styleBuilder">The builder for generating inline CSS from element properties.</param>
        /// <param name="eventExtractor">The extractor for converting XAML event handlers to HTML attributes.</param>
        /// <param name="controlRegistry">The registry of control-specific renderers.</param>
        /// <param name="behaviorRegistry">The registry of behavior handlers.</param>
        /// <param name="styleRegistry">The style registry for CSS deduplication (injected).</param>
        /// <param name="templateEngine">The template expansion engine (injected).</param>
        public HtmlRenderer(
            IElementTagMapper tagMapper,
            IEnumerable<ILayoutRenderer> layoutRenderers,
            IStyleBuilder styleBuilder,
            IEventExtractor eventExtractor,
            ControlRendererRegistry controlRegistry,
            BehaviorRegistry behaviorRegistry,
            IStyleRegistry styleRegistry,
            ITemplateEngine templateEngine)
        {
            v_TagMapper = tagMapper;
            v_LayoutRenderers = layoutRenderers;
            v_StyleBuilder = styleBuilder;
            v_EventExtractor = eventExtractor;
            v_ControlRegistry = controlRegistry;
            v_BehaviorRegistry = behaviorRegistry;
            v_StyleRegistry = styleRegistry;
            v_TemplateEngine = templateEngine;
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a full HTML document from the root IR element.
        /// Wraps the rendered body content with standard HTML boilerplate
        /// and a consolidated CSS style block.
        /// </summary>
        /// <param name="root">The root IR element representing the XAML tree.</param>
        /// <returns>A complete HTML document string.</returns>
        public string RenderDocument(IntermediateRepresentationElement root)
        {

            var bodyBuilder = new StringBuilder();
            RenderElement(root, bodyBuilder, 0, null, null);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\" />");
            sb.AppendLine("<title>XAML to HTML Output</title>");
            sb.AppendLine(v_StyleRegistry.GenerateStyleBlock());
            sb.AppendLine(VirtualizationStyleInjector.Build());
            sb.AppendLine("<script src=\"xaml-runtime.js\"></script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.Append(bodyBuilder.ToString());
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recursively renders an IR element and its children into HTML markup
        /// with appropriate indentation, CSS class assignment, binding attributes,
        /// event attributes, and special handling for input-type elements.
        /// </summary>
        /// <param name="element">The IR element to render.</param>
        /// <param name="sb">The string builder to append the rendered HTML to.</param>
        /// <param name="indent">The current indentation level in spaces.</param>
        /// <param name="parentLayoutType">The layout type name of the parent container, or null if none.</param>
        /// <param name="parentOrientation">The orientation of the parent container, or null if not applicable.</param>
        private void RenderElement(IntermediateRepresentationElement element, StringBuilder sb, int indent, string? parentLayoutType, string? parentOrientation)
        {
            try
            {
                var indentation = GetIndent(indent);
            var tag = ResolveTagMapping(element.Type);
            var style = BuildStyle(element, parentLayoutType, parentOrientation);

            sb.Append($"{indentation}<{tag}");
            var attributes = new AttributeBuffer();
            var controlRenderer = v_ControlRegistry.Resolve(element);

            // Apply control-specific attributes (via IAttributeRenderer)
            if (controlRenderer is IAttributeRenderer attributeRenderer)
            {
                attributeRenderer.RenderAttributes(element, attributes);
            }

            // Emit data-binding-* attributes
            var bindingAttributes = v_StyleBuilder.ExtractBindingAttributes(element);

            foreach (var attr in bindingAttributes)
            {
                attributes.Add(attr.Key, attr.Value);
            }

            // Emit data-event-* attributes
            var behaviors = v_BehaviorRegistry.Extract(element);

            foreach (var behavior in behaviors)
            {
                attributes.Add(behavior.Key, behavior.Value);
            }


            // Apply CSS class (deduplicated style)
            if (!string.IsNullOrWhiteSpace(style))
            {
                var className = v_StyleRegistry.Register(style);
                attributes.Add("class", className);
            }

            // Self-closing elements (input, img)
            if (tag == "input" || tag == "img")
            {
                if (element.Type == "Image" &&
                    element.Properties.TryGetValue("Source", out var src))
                {
                    attributes.Add("src", src);
                }

                attributes.WriteTo(sb);
                sb.Append(" />");

                if ((element.Type == "CheckBox" || element.Type == "RadioButton") &&
                    element.Properties.TryGetValue("Content", out var contentValue))
                {
                    sb.Append($" {contentValue}");
                }

                sb.AppendLine();
                return;
            }

            attributes.WriteTo(sb);
            sb.Append(">");
            
            // Apply control-specific content rendering (via IContentRenderer)
            if (controlRenderer is IContentRenderer contentRenderer)
            {
                contentRenderer.RenderContent(
                    element,
                    sb,
                    indent,
                    (child, builder, childIndent) =>
                    {
                        RenderElement(child, builder, childIndent, element.Type, parentOrientation);
                    });
            }
            else
            {
                // Default content rendering
                var content = ElementContentResolver.Resolve(element);

                if (!string.IsNullOrWhiteSpace(content))
                {
                    sb.Append(content);
                }

                // Render children recursively
                if (element.Children.Count > 0)
                {
                    sb.AppendLine();

                    for (int i = 0; i < element.Children.Count; i++)
                    {
                        var child = element.Children[i];

                        string? orientation = null;

                        if (element.Type == "StackPanel" &&
                            element.Properties.TryGetValue("Orientation", out var o))
                        {
                            orientation = o;
                        }

                        RenderElement(child, sb, indent + 2, element.Type, orientation);
                    }

                    sb.Append(indentation);
                }
            }

            sb.AppendLine($"</{tag}>");
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"Failed to render element '<{element.Type}>' at indent depth {indent}.",
                    ex);
            }

            
        }

        /// <summary>
        /// Builds the full inline CSS style string for an IR element by applying
        /// applicable layout renderers first, then appending property-based styles.
        /// </summary>
        /// <param name="element">The IR element to build styles for.</param>
        /// <param name="parentLayoutType">The layout type name of the parent container, or null.</param>
        /// <param name="parentOrientation">The orientation of the parent container, or null.</param>
        /// <returns>A combined CSS style string for the element.</returns>
        private string BuildStyle(IntermediateRepresentationElement element, string? parentLayoutType, string? parentOrientation)
        {
            var sb = new StringBuilder();

            // Apply layout container behavior (Grid, StackPanel, DockPanel, etc.)
            var renderer = ResolveLayoutRenderer(element);

            renderer?.ApplyLayout(element, sb);

            // Apply property-based styling (width, margin, alignment, grid positioning, etc.)
            var context = new LayoutContext(parentLayoutType, parentOrientation);
            sb.Append(v_StyleBuilder.Build(element, context));

            return sb.ToString();
        }
        private static string? ResolveElementContent(IntermediateRepresentationElement element)
        {
            if (!string.IsNullOrWhiteSpace(element.InnerText))
                return element.InnerText;

            if (element.Properties.TryGetValue("Text", out var text))
                return text;

            if (element.Properties.TryGetValue("Content", out var content))
                return content;

            return null;
        }

        /// <summary>
        /// Resolves the HTML tag for a XAML element type using cached lookup.
        /// Caches by element Type to avoid repeated tag mapper queries.
        /// Thread-safe: Uses ConcurrentDictionary.GetOrAdd for atomic cache operations.
        /// Performance optimization: eliminates repeated mapper lookups.
        /// </summary>
        private string ResolveTagMapping(string elementType)
        {
            // Thread-safe cache lookup and update with GetOrAdd
            return v_TagMappingCache.GetOrAdd(elementType, type =>
            {
                // Resolve tag from mapper only if not already cached
                return v_TagMapper.Map(type);
            });
        }

        /// <summary>
        /// Resolves the appropriate layout renderer for an element type using cached lookup.
        /// Caches by element Type to avoid repeated LINQ queries.
        /// Thread-safe: Uses ConcurrentDictionary.GetOrAdd for atomic cache operations.
        /// Performance optimization: eliminates Where/OrderByDescending enumerable allocations.
        /// </summary>
        private ILayoutRenderer? ResolveLayoutRenderer(IntermediateRepresentationElement element)
        {
            // Thread-safe cache lookup and update with GetOrAdd
            return v_LayoutRendererCache.GetOrAdd(element.Type, type =>
            {
                // No LINQ: Manual iteration with priority tracking
                ILayoutRenderer? result = null;
                int maxPriority = -1;

                foreach (var renderer in v_LayoutRenderers)
                {
                    if (renderer.CanHandle(element) && renderer.Priority > maxPriority)
                    {
                        result = renderer;
                        maxPriority = renderer.Priority;
                    }
                }

                // Return the result (even if null, to avoid re-checking)
                return result;
            });
        }

        internal void RenderChild(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        string? parentLayoutType,
        string? parentOrientation)
        {
            RenderElement(element, sb, indent, parentLayoutType, parentOrientation);
        }

        #endregion
    }
}
