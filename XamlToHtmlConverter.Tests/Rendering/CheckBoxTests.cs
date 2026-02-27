using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;
using System.Collections.Generic;

namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating CheckBox rendering behavior.
    /// Ensures correct HTML output and attribute mapping.
    /// </summary>
    public class CheckBoxTests
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
        /// Verifies that a CheckBox element is rendered
        /// as an input element with type="checkbox".
        /// </summary>
        [Fact]
        public void CheckBox_RendersAsInputCheckbox()
        {
            var checkBox = new IrElement("CheckBox");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(checkBox);

            Assert.Contains("<input", html);
            Assert.Contains("type=\"checkbox\"", html);
        }

        /// <summary>
        /// Verifies that IsChecked="True"
        /// results in a checked attribute in the output.
        /// </summary>
        [Fact]
        public void CheckBox_IsCheckedTrue_AddsCheckedAttribute()
        {
            var checkBox = new IrElement("CheckBox");
            checkBox.Properties.Add("IsChecked", "True");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(checkBox);

            Assert.Contains("checked", html);
        }

        /// <summary>
        /// Verifies that Content property is rendered
        /// as visible text alongside the checkbox.
        /// </summary>
        [Fact]
        public void CheckBox_WithContent_RendersContentText()
        {
            var checkBox = new IrElement("CheckBox");
            checkBox.Properties.Add("Content", "Remember me");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(checkBox);

            Assert.Contains("Remember me", html);
        }
    }
}
