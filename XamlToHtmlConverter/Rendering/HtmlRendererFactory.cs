// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.Rendering.Behavior;
using XamlToHtmlConverter.Rendering.Behavior.Handlers;
using XamlToHtmlConverter.Rendering.ControlRenderers;
using XamlToHtmlConverter.Rendering.Controls;
using XamlToHtmlConverter.Rendering.StyleMappers;
using XamlToHtmlConverter.Rendering.Templates;
using XamlToHtmlConverter.Rendering.LargeData;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Provides a centralized factory for creating a fully configured
    /// <see cref="HtmlRenderer"/> instance with all default dependencies wired.
    /// 
    /// Satisfies Dependency Inversion Principle:
    ///   - HtmlRenderer depends on abstractions (interfaces), not concretions
    ///   - StyleRegistry provided as IStyleRegistry dependency
    ///   - TemplateEngine provided as ITemplateEngine dependency
    ///   
    /// Satisfies Open/Closed Principle:
    ///   - Callers can extend by providing custom implementatio ns
    ///   - Factory methods accept optional injected dependencies
    /// </summary>
    public static class HtmlRendererFactory
    {
        #region Private Data

        /// <summary>
        /// Cached layout renderers collection. Prevents allocation on every Create() call.
        /// </summary>
        private static readonly ILayoutRenderer[] s_DefaultLayouts = new ILayoutRenderer[]
        {
            new GridLayoutRenderer(),
            new StackPanelLayoutRenderer(),
            new DockPanelLayoutRenderer(),
            new WrapPanelLayoutRenderer(),
            new ScrollViewerLayoutRenderer()
        };

        /// <summary>
        /// Cached control renderers collection.
        /// </summary>
        private static readonly IControlRenderer[] s_DefaultControlRenderers = new IControlRenderer[]
        {
            new TextBoxRenderer(),
            new CheckBoxRenderer(),
            new ListBoxRenderer(),
            new ItemsControlRenderer(),
            new RadioButtonRenderer(),
            new PasswordBoxRenderer(),
            new SliderRenderer(),
            new DatePickerRenderer(),
            new RichTextBoxRenderer(),
            new MediaElementRenderer(),
            new GroupBoxRenderer(),
            new ExpanderRenderer()
        };

        /// <summary>
        /// Cached behavior handlers collection.
        /// </summary>
        private static readonly IBehaviorHandler[] s_DefaultBehaviorHandlers = new IBehaviorHandler[]
        {
            new ClickBehavior(),
            new EnabledBehavior(),
            new VisibilityBehavior(),
            new CommandBehavior(),
            new CheckedBehavior(),
            new SelectedBehavior(),
            new TriggerBehavior()
        };

        /// <summary>
        /// Default property mappers for style building.
        /// </summary>
        private static readonly IPropertyMapper[] s_DefaultPropertyMappers = new IPropertyMapper[]
        {
            new WidthMapper(),
            new HeightMapper(),
            new TypographyMapper(),
            new BorderMapper(),
            new PaddingMapper(),
            new MarginMapper(),
            new MinMaxSizeMapper(),
            new AlignmentMapper(),
            new TextAlignmentMapper()
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates and returns an <see cref="HtmlRenderer"/> configured with the default
        /// tag mapper, all standard layout renderers, the default style builder,
        /// and the default event extractor.
        /// </summary>
        /// <returns>A fully initialized <see cref="HtmlRenderer"/> instance.</returns>
        public static HtmlRenderer Create()
        {
            return Create(
                tagMapperOverrides: Array.Empty<KeyValuePair<string, string>>(),
                extraControlRenderers: Array.Empty<IControlRenderer>(),
                extraLayoutRenderers: Array.Empty<ILayoutRenderer>(),
                extraPropertyMappers: Array.Empty<IPropertyMapper>()
            );
        }

        /// <summary>
        /// Creates an <see cref="HtmlRenderer"/> with optional custom extensions.
        /// Enables clients to extend the renderer without modifying the factory (Open/Closed Principle).
        /// </summary>
        /// <param name="tagMapperOverrides">Custom XAML-to-HTML tag mappings.</param>
        /// <param name="extraControlRenderers">Additional control renderers beyond defaults.</param>
        /// <param name="extraLayoutRenderers">Additional layout renderers beyond defaults.</param>
        /// <param name="extraPropertyMappers">Additional property mappers for style building.</param>
        /// <returns>A fully configured <see cref="HtmlRenderer"/> instance.</returns>
        public static HtmlRenderer Create(
            IEnumerable<KeyValuePair<string, string>>? tagMapperOverrides = null,
            IEnumerable<IControlRenderer>? extraControlRenderers = null,
            IEnumerable<ILayoutRenderer>? extraLayoutRenderers = null,
            IEnumerable<IPropertyMapper>? extraPropertyMappers = null)
        {
            // Prepare tag mapper with overrides (Open/Closed Principle)
            var tagMapper = new DefaultElementTagMapper(tagMapperOverrides ?? Array.Empty<KeyValuePair<string, string>>());

            // Prepare layout renderers
            var layoutRenderers = s_DefaultLayouts.Concat(extraLayoutRenderers ?? Array.Empty<ILayoutRenderer>());

            // Prepare property mappers
            var propertyMappers = s_DefaultPropertyMappers.Concat(extraPropertyMappers ?? Array.Empty<IPropertyMapper>());

            // Create style builder with injected mappers (Dependency Inversion Principle)
            var styleBuilder = new DefaultStyleBuilder(propertyMappers);

            // Create registries
            var controlRegistry = new ControlRendererRegistry(
                s_DefaultControlRenderers.Concat(extraControlRenderers ?? Array.Empty<IControlRenderer>())
            );
            var behaviorRegistry = new BehaviorRegistry(s_DefaultBehaviorHandlers);

            // Create abstraction implementations
            var styleRegistry = new StyleRegistry();
            var templateEngine = new TemplateEngine();

            return new HtmlRenderer(
                tagMapper,
                layoutRenderers,
                styleBuilder,
                new DefaultEventExtractor(),
                controlRegistry,
                behaviorRegistry,
                styleRegistry,
                templateEngine
            );
        }

        #endregion
    }
}

