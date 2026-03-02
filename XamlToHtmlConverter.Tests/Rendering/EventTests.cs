using System;
using System.Collections.Generic;
using XamlToHtmlConverter.Rendering;
using XamlToHtmlConverter.IR;
using Xunit;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating event metadata extraction.
    /// Ensures supported XAML events are converted to HTML data-event attributes.
    /// </summary>
    public class EventTests
    {
        /// <summary>
        /// Creates a configured HtmlRenderer instance
        /// including event extraction support.
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
        /// Verifies that a Button with a Click event
        /// generates the corresponding data-event-click attribute.
        /// </summary>
        [Fact]
        public void Button_WithClickEvent_PreservesEventMetadata()
        {
            var button = new IrElement("Button");
            button.Properties.Add("Click", "OnSubmit");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(button);

            Assert.Contains("data-event-click=\"OnSubmit\"", html);
        }
        /// <summary>
        /// Verifies that a TextBox with a TextChanged event
        /// generates the corresponding data-event-textchanged attribute.
        /// </summary>
        [Fact]
        public void TextBox_WithTextChangedEvent_PreservesEventMetadata()
        {
            var textBox = new IrElement("TextBox");
            textBox.Properties.Add("TextChanged", "OnTextChanged");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(textBox);

            Assert.Contains("data-event-textchanged=\"OnTextChanged\"", html);
        }
    }

}
