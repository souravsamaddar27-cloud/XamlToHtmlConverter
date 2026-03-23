// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for PasswordBox elements, mapping to HTML input[type=password].
/// </summary>
public class PasswordBoxRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "PasswordBox";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // PasswordBox has no child content
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Set type attribute to password
        attributes.Add("type", "password");

        // Handle MaxLength property
        if (element.Properties.TryGetValue("MaxLength", out var maxLength))
            attributes.Add("maxlength", maxLength);

        // Handle IsReadOnly property
        if (element.Properties.TryGetValue("IsReadOnly", out var isReadOnly)
            && isReadOnly.Equals("True", StringComparison.OrdinalIgnoreCase))
            attributes.Add("readonly", "");

        // Handle bindings if any
        if (element.Bindings.TryGetValue("Password", out var binding))
            attributes.Add("data-binding-password", binding.Path);
    }
}
