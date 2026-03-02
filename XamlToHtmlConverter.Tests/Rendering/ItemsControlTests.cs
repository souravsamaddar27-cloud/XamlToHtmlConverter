using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;
namespace XamlToHtmlConverter.Tests.Rendering
{
    /// <summary>
    /// Contains unit tests validating ItemsControl rendering behavior.
    /// Ensures correct handling of static items, items containers,
    /// bindings, and data templates.
    /// </summary>
    public class ItemsControlTests
    {
     /// <summary>
     /// Creates a configured HtmlRenderer instance
     /// including layout, styling, and event extraction support.
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
        /// Verifies that static child elements inside ItemsControl
        /// are rendered as normal children.
        /// </summary>
        [Fact]
        public void ItemsControl_WithStaticItems_RendersChildren()
        {
            var itemsControl = new IrElement("ItemsControl");

            var item1 = new IrElement("TextBlock");
            item1.InnerText = "One";

            var item2 = new IrElement("TextBlock");
            item2.InnerText = "Two";

            itemsControl.Children.Add(item1);
            itemsControl.Children.Add(item2);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(itemsControl);

            Assert.Contains(">One<", html);
            Assert.Contains(">Two<", html);
        }
        /// <summary>
        /// Verifies that elements inside ItemsControl.Items
        /// are rendered as items while the container element itself is omitted.
        /// </summary>
        [Fact]
        public void ItemsControl_WithItemsProperty_RendersItemChildren()
        {
            var itemsControl = new IrElement("ItemsControl");

            var itemsContainer = new IrElement("ItemsControl.Items");

            var item1 = new IrElement("TextBlock");
            item1.InnerText = "One";

            var item2 = new IrElement("TextBlock");
            item2.InnerText = "Two";

            itemsContainer.Children.Add(item1);
            itemsContainer.Children.Add(item2);

            itemsControl.Children.Add(itemsContainer);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(itemsControl);

            Assert.Contains(">One<", html);
            Assert.Contains(">Two<", html);
            Assert.DoesNotContain("ItemsControl.Items", html);
        }
        /// <summary>
        /// Verifies that ItemsSource binding is preserved
        /// as data-binding metadata.
        /// </summary>
        [Fact]
        public void ItemsControl_WithItemsSource_PreservesBindingMetadata()
        {
            var itemsControl = new IrElement("ItemsControl");
            itemsControl.Properties.Add("ItemsSource", "{Binding Users}");

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(itemsControl);

            Assert.Contains("data-binding-itemssource=\"Users\"", html);
        }
        /// <summary>
        /// Verifies that a DataTemplate inside ItemsControl
        /// is rendered as a template element and preserves inner bindings.
        /// </summary>
        [Fact]
        public void ItemsControl_WithDataTemplate_RendersTemplateElement()
        {
            var itemsControl = new IrElement("ItemsControl");
            itemsControl.Properties.Add("ItemsSource", "{Binding Users}");

            var templateContainer = new IrElement("ItemsControl.ItemTemplate");
            var dataTemplate = new IrElement("DataTemplate");

            var textBlock = new IrElement("TextBlock");
            textBlock.Properties.Add("Text", "{Binding Name}");

            dataTemplate.Children.Add(textBlock);
            templateContainer.Children.Add(dataTemplate);
            itemsControl.Children.Add(templateContainer);

            var renderer = CreateRenderer();
            var html = renderer.RenderDocument(itemsControl);

            Assert.Contains("<template>", html);
            Assert.Contains("data-binding-text=\"Name\"", html);
        }
    }
}
