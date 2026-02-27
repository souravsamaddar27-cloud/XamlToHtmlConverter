using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;
using System.Collections.Generic;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating Z-index handling.
    /// Ensures Panel.ZIndex is correctly mapped to CSS z-index.
    /// </summary>
    public class ZIndexTests
    {
        /// <summary>
        /// Creates a configured HtmlRenderer instance
        /// with layout renderers and default style builder.
        /// </summary>
        private HtmlRenderer CreateRenderer()
        {
            return new HtmlRenderer(
                new DefaultElementTagMapper(),
                new ILayoutRenderer[]
                {
                    new GridLayoutRenderer(),
                    new StackPanelLayoutRenderer()
                },
                new DefaultStyleBuilder());
        }
        /// <summary>
        /// Verifies that the attached property Panel.ZIndex
        /// is translated to a z-index CSS style.
        /// </summary>
        [Fact]
        public void PanelZIndex_MapsToZIndexCss()
        {
            var button = new IrElement("Button");
            button.AttachedProperties.Add("Panel.ZIndex", "5");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            Assert.Contains("z-index:5;", html);
        }
    }
}
