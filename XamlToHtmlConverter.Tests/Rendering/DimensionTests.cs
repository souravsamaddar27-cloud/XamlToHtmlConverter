using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;
using System.Collections.Generic;


namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating dimension-related properties.
    /// Ensures min/max width and height are correctly mapped to CSS.
    /// </summary>
    public class DimensionTests
    {
        /// <summary>
        /// Creates a configured HtmlRenderer instance
        /// with required layout and styling components.
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
        /// Verifies that MinWidth property maps to min-width CSS.
        /// </summary>
        [Fact]
            public void MinWidth_MapsToMinWidthCss()
            {
                var button = new IrElement("Button");
                button.Properties.Add("MinWidth", "100");

                var renderer = CreateRenderer();
                var html = renderer.RenderDocument(button);

                Assert.Contains("min-width:100px;", html);
            }
        /// <summary>
        /// Verifies that MaxWidth property maps to max-width CSS.
        /// </summary>
        [Fact]
            public void MaxWidth_MapsToMaxWidthCss()
            {
                var button = new IrElement("Button");
                button.Properties.Add("MaxWidth", "300");

                var renderer = CreateRenderer();
                var html = renderer.RenderDocument(button);

                Assert.Contains("max-width:300px;", html);
            }

        /// <summary>
        /// Verifies that MinHeight property maps to min-height CSS.
        /// </summary>
        [Fact]
            public void MinHeight_MapsToMinHeightCss()
            {
                var button = new IrElement("Button");
                button.Properties.Add("MinHeight", "50");

                var renderer = CreateRenderer();
                var html = renderer.RenderDocument(button);

                Assert.Contains("min-height:50px;", html);
            }

        /// <summary>
        /// Verifies that MaxHeight property maps to max-height CSS.
        /// </summary>
        [Fact]
            public void MaxHeight_MapsToMaxHeightCss()
            {
                var button = new IrElement("Button");
                button.Properties.Add("MaxHeight", "200");

                var renderer = CreateRenderer();
                var html = renderer.RenderDocument(button);

                Assert.Contains("max-height:200px;", html);
            }
        }
    }
