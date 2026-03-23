// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.ControlRenderers;
using XamlToHtmlConverter.Rendering.LargeData;

namespace XamlToHtmlConverter.Rendering.Controls
{
    public class ItemsControlRenderer : IControlRenderer
    {
        public bool CanHandle(IntermediateRepresentationElement element)
        {
            return element.Type == "ItemsControl"
                || element.Type == "ListView"
                || element.Type == "ListBox"
                || element.Type == "ComboBox";
        }

        public void RenderAttributes(
                IntermediateRepresentationElement element,
                AttributeBuffer attributes)
            {
                if (element.Type == "ListBox")
                    attributes.Add("multiple", "");

                if (element.Bindings.TryGetValue("ItemsSource", out var binding)
                    && !string.IsNullOrEmpty(binding?.Path))
                    attributes.Add("data-binding-itemssource", binding.Path!);

                if (element.Bindings.TryGetValue("SelectedIndex", out var si)
                    && !string.IsNullOrEmpty(si?.Path))
                    attributes.Add("data-binding-selectedindex", si.Path!);

                if (element.Bindings.TryGetValue("SelectedItem", out var sit)
                    && !string.IsNullOrEmpty(sit?.Path))
                    attributes.Add("data-binding-selecteditem", sit.Path!);

                if (element.Bindings.TryGetValue("SelectedValue", out var sv)
                    && !string.IsNullOrEmpty(sv?.Path))
                    attributes.Add("data-binding-selectedvalue", sv.Path!);

                if (element.Properties.TryGetValue("DisplayMemberPath", out var dmp))
                    attributes.Add("data-display-member", dmp);

                if (element.Type == "ComboBox"
                    && element.Properties.TryGetValue("IsEditable", out var editable)
                    && editable.Equals("True", StringComparison.OrdinalIgnoreCase))
                {
                    attributes.Add("data-combobox-editable", "true");
                }
            }

        public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
        {
            // Phase 4 virtualization
            if (VirtualizationDetector.RequiresVirtualization(element))
            {
                VirtualizedItemsRenderer.RenderPlaceholder(element, sb, indent);
                return;
            }

            // Normal ItemsControl behavior
            if (element.ItemTemplate != null)
            {
                var indentation = new string(' ', indent + 2);

                sb.AppendLine();
                sb.AppendLine($"{indentation}<div>");

                renderChild(element.ItemTemplate, sb, indent + 4);

                sb.AppendLine($"{indentation}</div>");
            }
        }
    }
}