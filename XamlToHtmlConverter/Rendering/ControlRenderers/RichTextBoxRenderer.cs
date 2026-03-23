// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for RichTextBox elements, mapping to HTML div[contenteditable=true].
/// </summary>
public class RichTextBoxRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "RichTextBox";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // RichTextBox renders its inner text/content
        if (!string.IsNullOrEmpty(element.InnerText))
        {
            var indentation = new string(' ', indent + 2);
            sb.AppendLine($"{indentation}{System.Net.WebUtility.HtmlEncode(element.InnerText)}");
        }
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Set contenteditable attribute to allow editing
        attributes.Add("contenteditable", "true");

        // Handle IsReadOnly property
        if (element.Properties.TryGetValue("IsReadOnly", out var isReadOnly)
            && isReadOnly.Equals("True", StringComparison.OrdinalIgnoreCase))
            attributes.Add("contenteditable", "false");

        // Handle bindings if any
        if (element.Bindings.TryGetValue("Document", out var binding)
            && !string.IsNullOrEmpty(binding?.Path))
            attributes.Add("data-binding-document", binding.Path!);

        // Ensure role is set for accessibility
        attributes.Add("role", "textbox");
    }
}
