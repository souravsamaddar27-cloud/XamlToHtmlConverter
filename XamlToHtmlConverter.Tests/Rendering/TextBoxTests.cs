using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.Rendering;
using Xunit;
using XamlToHtmlConverter.IR;


namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating TextBox rendering behavior.
    /// Ensures correct HTML input mapping and value handling.
    /// </summary>
    public class TextBoxTests
    {
        /// <summary>
        /// Creates a configured HtmlRenderer instance
        /// including layout renderers and style builder.
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
        /// Verifies that a TextBox renders as an input element
        /// with type="text".
        /// </summary>
        [Fact]
        public void TextBox_RendersAsInputText()
        {
            var textBox = new IrElement("TextBox");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(textBox);

            Assert.Contains("<input", html);
            Assert.Contains("type=\"text\"", html);
        }

        /// <summary>
        /// Verifies that when a Text property is provided,
        /// it is emitted as the value attribute in the output.
        /// </summary>
        [Fact]
        public void TextBox_WithText_SetsValueAttribute()
        {
            var textBox = new IrElement("TextBox");
            textBox.Properties.Add("Text", "Hello");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(textBox);

            Assert.Contains("value=\"Hello\"", html);
        }
    }
}
