using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;
using System.Collections.Generic;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating rendering behavior
    /// for additional UI controls and special cases.
    /// </summary>
    public class AdditionalControlsTests
    {
        /// <summary>
        /// Creates a configured HtmlRenderer instance
        /// with default tag mapping, layout renderers, and style builder.
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
        /// Verifies that a RadioButton is rendered as
        /// an input element with type="radio".
        /// </summary>
        [Fact]
        public void RadioButton_RendersAsInputRadio()
        {
            var radio = new IrElement("RadioButton");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(radio);

            Assert.Contains("type=\"radio\"", html);
        }
        /// <summary>
        /// Verifies that an Image element is rendered as an img tag
        /// with the correct source attribute.
        /// </summary>
        [Fact]
        public void Image_RendersAsImgWithSource()
        {
            var image = new IrElement("Image");
            image.Properties.Add("Source", "logo.png");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(image);

            Assert.Contains("<img", html);
            Assert.Contains("src=\"logo.png\"", html);
        }
        /// <summary>
        /// Verifies that a ComboBox is rendered as a select element.
        /// </summary>
        [Fact]
        public void ComboBox_RendersAsSelect()
        {
            var combo = new IrElement("ComboBox");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(combo);

            Assert.Contains("<select", html);
        }
        /// <summary>
        /// Verifies that a ListBox is rendered as a select element.
        /// </summary>
        [Fact]
        public void ListBox_RendersAsSelectMultiple()
        {
            var list = new IrElement("ListBox");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(list);

            Assert.Contains("<select", html);
        }
        /// <summary>
        /// Verifies that ComboBox items are rendered as option elements.
        /// </summary>
        [Fact]
        public void ComboBox_WithItems_RendersOptions()
        {
            var combo = new IrElement("ComboBox");

            var item1 = new IrElement("ComboBoxItem");
            item1.InnerText = "One";

            var item2 = new IrElement("ComboBoxItem");
            item2.InnerText = "Two";

            combo.Children.Add(item1);
            combo.Children.Add(item2);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(combo);

            Assert.Contains("<option>One</option>", html);
            Assert.Contains("<option>Two</option>", html);
        }
        /// <summary>
        /// Verifies that a TextBox with binding does not emit a value attribute
        /// and instead preserves binding metadata.
        /// </summary>
        [Fact]
        public void TextBox_WithBinding_DoesNotEmitValueAttribute()
        {
            var textBox = new IrElement("TextBox");
            textBox.Properties.Add("Text", "{Binding UserName}");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(textBox);

            Assert.Contains("data-binding-text=\"UserName\"", html);
            Assert.DoesNotContain("value=\"", html);
        }
        /// <summary>
        /// Verifies that a ContentControl renders its Content
        /// as inner text within the generated HTML element.
        /// </summary>
        [Fact]
        public void ContentControl_WithContent_RendersInnerText()
        {
            var control = new IrElement("ContentControl");
            control.Properties.Add("Content", "Hello");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(control);

            Assert.Contains(">Hello</div>", html);
        }
    }
}
