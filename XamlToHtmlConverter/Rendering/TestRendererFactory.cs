using System;
using System.Collections.Generic;
using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Provides a centralized factory for creating
    /// a fully configured HtmlRenderer instance for testing.
    /// </summary>
    public static class TestRendererFactory
    {
        /// <summary>
        /// Creates an HtmlRenderer with default tag mapper,
        /// layout renderers, style builder, and event extractor.
        /// </summary>
        public static HtmlRenderer Create()
        {
            var layouts = new List<ILayoutRenderer>
        {
            new GridLayoutRenderer(),
            new StackPanelLayoutRenderer(),
            new DockPanelLayoutRenderer(),
            new WrapPanelLayoutRenderer()
        };

            return new HtmlRenderer(
                new DefaultElementTagMapper(),
                layouts,
                new DefaultStyleBuilder(),
                new DefaultEventExtractor()
            );
        }
    }
}
