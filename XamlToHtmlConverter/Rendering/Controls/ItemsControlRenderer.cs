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
            {
                attributes.Add("multiple", string.Empty);
            }

            if (element.Bindings.TryGetValue("ItemsSource", out var binding))
            {
                attributes.Add("data-binding-itemssource", binding.Path ?? string.Empty);
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