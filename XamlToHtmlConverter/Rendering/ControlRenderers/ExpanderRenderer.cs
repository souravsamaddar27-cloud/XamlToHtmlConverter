// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for Expander elements, mapping to HTML details with injected summary.
/// </summary>
public class ExpanderRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "Expander";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // Render the summary (Header property becomes the summary text)
        if (element.Properties.TryGetValue("Header", out var header))
        {
            var summaryIndent = new string(' ', indent + 2);
            sb.AppendLine($"{summaryIndent}<summary>{System.Net.WebUtility.HtmlEncode(header)}</summary>");
        }

        // Render child content
        foreach (var child in element.Children)
        {
            renderChild(child, sb, indent + 2);
        }
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Handle IsExpanded property by setting open attribute on details
        if (element.Properties.TryGetValue("IsExpanded", out var isExpanded)
            && isExpanded.Equals("True", StringComparison.OrdinalIgnoreCase))
            attributes.Add("open", "");
    }
}
