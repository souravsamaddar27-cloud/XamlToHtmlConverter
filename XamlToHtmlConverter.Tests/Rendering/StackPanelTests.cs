using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;
using System.Collections.Generic;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating StackPanel layout behavior.
    /// Ensures orientation is correctly translated to flexbox direction.
    /// </summary>
    public class StackPanelTests
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
                new DefaultStyleBuilder(), new DefaultEventExtractor());
        }

        /// <summary>
        /// Verifies that a StackPanel with Horizontal orientation
        /// generates a flexbox row direction in CSS.
        /// </summary>
        [Fact]
        public void StackPanel_WithHorizontalOrientation_UsesRowFlex()
        {
            var stack = new IrElement("StackPanel");
            stack.Properties["Orientation"] = "Horizontal";

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(stack);

            Assert.Contains("flex-direction:row;", html);
        }
    }
}