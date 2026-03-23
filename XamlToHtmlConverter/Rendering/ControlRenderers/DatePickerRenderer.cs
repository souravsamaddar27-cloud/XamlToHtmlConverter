// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for DatePicker elements, mapping to HTML input[type=date].
/// </summary>
public class DatePickerRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "DatePicker";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // DatePicker has no child content
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Set type attribute to date
        attributes.Add("type", "date");

        // Handle SelectedDate property
        if (element.Properties.TryGetValue("SelectedDate", out var selectedDate))
            attributes.Add("value", selectedDate);

        // Handle DisplayDateStart (minimum date)
        if (element.Properties.TryGetValue("DisplayDateStart", out var minDate))
            attributes.Add("min", minDate);

        // Handle DisplayDateEnd (maximum date)
        if (element.Properties.TryGetValue("DisplayDateEnd", out var maxDate))
            attributes.Add("max", maxDate);

        // Handle IsEnabled property
        if (element.Properties.TryGetValue("IsEnabled", out var isEnabled)
            && isEnabled.Equals("False", StringComparison.OrdinalIgnoreCase))
            attributes.Add("disabled", "");

        // Handle SelectedDate binding
        if (element.Bindings.TryGetValue("SelectedDate", out var binding)
            && !string.IsNullOrEmpty(binding?.Path))
            attributes.Add("data-binding-selecteddate", binding.Path!);
    }
}
