// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for GroupBox elements, mapping to HTML fieldset with injected legend.
/// </summary>
public class GroupBoxRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "GroupBox";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // Render the legend (Header property becomes the legend text)
        if (element.Properties.TryGetValue("Header", out var header))
        {
            var indentation = new string(' ', indent + 2);
            sb.AppendLine($"{indentation}<legend>{System.Net.WebUtility.HtmlEncode(header)}</legend>");
        }

        // Render child content
        foreach (var child in element.Children)
        {
            renderChild(child, sb, indent + 2);
        }
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // GroupBox doesn't need special attributes beyond semantic fieldset
        // All styling should be handled by the style builder
    }
}
