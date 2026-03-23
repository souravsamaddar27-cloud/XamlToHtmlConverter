// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for Slider elements, mapping to HTML input[type=range].
/// </summary>
public class SliderRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "Slider";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // Slider has no child content
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Set type attribute to range
        attributes.Add("type", "range");

        // Handle Minimum property
        if (element.Properties.TryGetValue("Minimum", out var minimum))
            attributes.Add("min", minimum);

        // Handle Maximum property
        if (element.Properties.TryGetValue("Maximum", out var maximum))
            attributes.Add("max", maximum);

        // Handle Value property
        if (element.Properties.TryGetValue("Value", out var value))
            attributes.Add("value", value);

        // Handle SmallChange (step) property
        if (element.Properties.TryGetValue("SmallChange", out var step))
            attributes.Add("step", step);

        // Handle IsEnabled property
        if (element.Properties.TryGetValue("IsEnabled", out var isEnabled)
            && isEnabled.Equals("False", StringComparison.OrdinalIgnoreCase))
            attributes.Add("disabled", "");

        // Handle Value binding
        if (element.Bindings.TryGetValue("Value", out var binding)
            && !string.IsNullOrEmpty(binding?.Path))
            attributes.Add("data-binding-value", binding.Path!);
    }
}
