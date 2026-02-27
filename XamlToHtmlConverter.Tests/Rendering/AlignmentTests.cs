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
        /// <summary>
        /// Verifies that when no alignment is specified in a Grid,
        /// no alignment styles are emitted.
        /// </summary>
        [Fact]
        public void Button_InGrid_WithNoAlignment_DefaultsToStretch()
        {
            var grid = new IrElement("Grid");
            grid.GridRowDefinitions.Add("*");

            var button = new IrElement("Button");
            button.InnerText = "Test";

            grid.Children.Add(button);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(grid);

            Assert.DoesNotContain("justify-self", html);
            Assert.DoesNotContain("align-self", html);
        }
        /// <summary>
        /// Verifies that HorizontalAlignment inside a vertical StackPanel
        /// maps to align-self.
        /// </summary>
        [Fact]
        public void VerticalStackPanel_HorizontalAlignment_MapsToAlignSelf()
        {
            var stack = new IrElement("StackPanel");
            stack.Properties.Add("Orientation", "Vertical");

            var button = new IrElement("Button");
            button.Properties.Add("HorizontalAlignment", "Center");

            stack.Children.Add(button);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(stack);

            Assert.Contains("align-self:center;", html);
        }
        /// <summary>
        /// Verifies that VerticalAlignment inside a horizontal StackPanel
        /// maps to align-self.
        /// </summary>
        [Fact]
        public void HorizontalStackPanel_VerticalAlignment_MapsToAlignSelf()
        {
            var stack = new IrElement("StackPanel");
            stack.Properties.Add("Orientation", "Horizontal");

            var button = new IrElement("Button");
            button.Properties.Add("VerticalAlignment", "Top");

            stack.Children.Add(button);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(stack);

            Assert.Contains("align-self:start;", html);
        }

        /// <summary>
        /// Verifies that Visibility="Collapsed"
        /// maps to display:none.
        /// </summary>
        [Fact]
        public void Visibility_Collapsed_MapsToDisplayNone()
        {
            var button = new IrElement("Button");
            button.Properties.Add("Visibility", "Collapsed");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            Assert.Contains("display:none;", html);
        }

        /// <summary>
        /// Verifies that Visibility="Hidden"
        /// maps to visibility:hidden.
        /// </summary>
        [Fact]
        public void Visibility_Hidden_MapsToVisibilityHidden()
        {
            var button = new IrElement("Button");
            button.Properties.Add("Visibility", "Hidden");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            Assert.Contains("visibility:hidden;", html);
        }

        /// <summary>
        /// Verifies that Visibility="Visible"
        /// does not produce any visibility-related styles.
        /// </summary>
        [Fact]
        public void Visibility_Visible_ProducesNoVisibilityStyle()
        {
            var button = new IrElement("Button");
            button.Properties.Add("Visibility", "Visible");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            Assert.DoesNotContain("display:none;", html);
            Assert.DoesNotContain("visibility:hidden;", html);
        }
    }
}
