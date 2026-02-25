using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating Grid span behavior.
    /// Ensures column span properties are correctly translated to CSS.
    /// </summary>
    public class GridSpanTests
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
        /// Verifies that an element with Grid.Column and Grid.ColumnSpan
        /// generates the correct CSS grid-column span syntax.
        /// </summary>
        [Fact]
        public void Element_WithColumnSpan_GeneratesCorrectCss()
        {
            var grid = new IrElement("Grid");
            grid.GridColumnDefinitions.Add("*");
            grid.GridColumnDefinitions.Add("*");

            var child = new IrElement("Border");
            child.AttachedProperties["Grid.Column"] = "0";
            child.AttachedProperties["Grid.ColumnSpan"] = "2";

            grid.Children.Add(child);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(grid);

            Assert.Contains("grid-column:1 / span 2;", html);
        }
    }
}