using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;
using System.Collections.Generic;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating DockPanel layout behavior.
    /// Ensures correct flexbox rendering and dock direction handling.
    /// </summary>
    public class DockPanelTests
    {
        /// <summary>
        /// Creates a configured HtmlRenderer instance
        /// including DockPanel layout support.
        /// </summary>

        private HtmlRenderer CreateRenderer()
        {
            return new HtmlRenderer(
                new DefaultElementTagMapper(),
                new ILayoutRenderer[]
                {
            new GridLayoutRenderer(),
            new StackPanelLayoutRenderer(),
            new DockPanelLayoutRenderer()
                },
                new DefaultStyleBuilder(), new DefaultEventExtractor());
        }
        /// <summary>
        /// Verifies that a DockPanel renders as a flex container.
        /// </summary>
        [Fact]
        public void DockPanel_RendersAsFlexContainer()
        {
            var dock = new IrElement("DockPanel");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(dock);

            Assert.Contains("display:flex;", html);
        }

        /// <summary>
        /// Verifies that a child with DockPanel.Dock="Top"
        /// results in column flex direction for layout.
        /// </summary>
        [Fact]
        public void DockPanel_TopDock_AddsColumnDirection()
        {
            var dock = new IrElement("DockPanel");

            var child = new IrElement("Button");
            child.AttachedProperties.Add("DockPanel.Dock", "Top");

            dock.Children.Add(child);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(dock);

            Assert.Contains("flex-direction:column;", html);
        }
    }
}
