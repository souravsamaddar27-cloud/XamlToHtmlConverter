using System;
using System.Collections.Generic;
using XamlToHtmlConverter.Rendering;
using XamlToHtmlConverter.IR;
using Xunit;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating Grid layout rendering behavior.
    /// Ensures correct CSS grid template generation.
    /// </summary>
    public class GridLayoutRendererTests
    {
        /// <summary>
        /// Creates a configured HtmlRenderer instance
        /// with required layout and styling components.
        /// </summary>
        private HtmlRenderer CreateRenderer()
        {
            return new HtmlRenderer(new DefaultElementTagMapper(), new ILayoutRenderer[]
            {
                new GridLayoutRenderer(),
                new StackPanelLayoutRenderer()
            },
            new DefaultStyleBuilder(), new DefaultEventExtractor());
        }
        /// <summary>
        /// Verifies that Grid row definitions containing
        /// Auto and star sizing are correctly converted
        /// to CSS grid-template-rows syntax.
        /// </summary>
        [Fact]
         public void Grid_WithAutoAndStartRows_GeneratesCorrectTemplate()
        {
            var grid = new IrElement("Grid");
            grid.GridRowDefinitions.Add("Auto");
            grid.GridRowDefinitions.Add("*");

            var renderer=CreateRenderer();
            var html = renderer.RenderDocument(grid);
            Assert.Contains("grid-template-rows:auto 1fr;", html);

        }
    }
}
