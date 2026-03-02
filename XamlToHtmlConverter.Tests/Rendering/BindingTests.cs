using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;
using System.Collections.Generic;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating XAML binding handling.
    /// Ensures binding expressions are correctly extracted
    /// and preserved as HTML metadata attributes.
    /// </summary>
    public class BindingTests
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
                new DefaultStyleBuilder(),
                new DefaultEventExtractor());
        }

        /// <summary>
        /// Verifies that a simple binding expression
        /// is converted into a corresponding data-binding attribute.
        /// </summary>
        [Fact]
        public void Button_WithSimpleBinding_PreservesBindingMetadata()
        {
            var button = new IrElement("Button");
            button.Properties.Add("Content", "{Binding SubmitText}");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            Assert.Contains("data-binding-content=\"SubmitText\"", html);
        }
        /// <summary>
        /// Verifies that binding expressions using Path syntax
        /// correctly extract and preserve the binding path.
        /// </summary>
        [Fact]
        public void Binding_WithPathSyntax_ExtractsPath()
        {
            var button = new IrElement("Button");
            button.Properties.Add("Content", "{Binding Path=UserName}");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            Assert.Contains("data-binding-content=\"UserName\"", html);
        }

        /// <summary>
        /// Verifies that additional binding options
        /// do not interfere with correct path extraction.
        /// </summary>
        [Fact]
        public void Binding_WithModeOption_StillExtractsPath()
        {
            var button = new IrElement("Button");
            button.Properties.Add("Content", "{Binding Path=UserName, Mode=TwoWay}");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            Assert.Contains("data-binding-content=\"UserName\"", html);
        }
        /// <summary>
        /// Verifies that multiple binding properties are preserved
        /// and that normal non-binding properties remain unaffected.
        /// </summary>
        [Fact]
        public void Element_WithMultipleBindings_PreservesEach_And_LeavesNormalPropertiesUntouched()
        {
            var button = new IrElement("Button");
            button.Properties.Add("Content", "{Binding SubmitText}");
            button.Properties.Add("ToolTip", "{Binding TooltipText}");
            button.Properties.Add("Width", "150"); // normal property

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            // Binding metadata should exist
            Assert.Contains("data-binding-content=\"SubmitText\"", html);
            Assert.Contains("data-binding-tooltip=\"TooltipText\"", html);

            // Normal property should still render as style
            Assert.Contains("width:150px;", html);
        }
    }
}