using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.Rendering;
using XamlToHtmlConverter.IR;
using Xunit;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests verifying alignment behavior
    /// when rendering elements within different layout containers.
    /// </summary>

    public class AlignmentTests
    {
        /// <summary>
        /// Creates a configured HtmlRenderer instance
        /// with default tag mapping, layout renderers, and style builder.
        /// </summary>
        private HtmlRenderer CreateRenderer()
        {
            return new HtmlRenderer (
                new DefaultElementTagMapper(),
                new ILayoutRenderer[]
                {
                    new GridLayoutRenderer(),
                    new StackPanelLayoutRenderer()
                    },
                        new DefaultStyleBuilder());
        }
        /// <summary>
        /// Verifies that alignment properties applied to a Button
        /// inside a Grid are correctly translated to CSS grid alignment styles.
        /// </summary>
        [Fact]
        public void Button_InGrid_WithAlignment_MapsCorrectly()
        {
            var grid = new IrElement("Grid");
            grid.GridRowDefinitions.Add("*");

            var button = new IrElement("Button");
            button.Properties.Add("HorizontalAlignment", "Center");
            button.Properties.Add("VerticalAlignment", "Top");
            button.InnerText = "Test";
            grid.Children.Add(button);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(grid);
            //Console.WriteLine(html);
            //throw new Exception(html);
            Assert.Contains("justify-self:center;", html);
            Assert.Contains("align-self:start;", html);
        }
    }
}
