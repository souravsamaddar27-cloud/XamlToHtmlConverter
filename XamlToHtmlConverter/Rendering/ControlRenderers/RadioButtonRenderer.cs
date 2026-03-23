using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

public class RadioButtonRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "RadioButton";

    public void RenderAttributes(IntermediateRepresentationElement element,
        AttributeBuffer attributes)
    {
        attributes.Add("type", "radio");

        // GroupName is critical for grouping
        if (element.Properties.TryGetValue("GroupName", out var group))
            attributes.Add("name", group);

        if (element.Bindings.TryGetValue("IsChecked", out var binding)
            && !string.IsNullOrEmpty(binding?.Path))
            attributes.Add("data-binding-checked", binding.Path!);
    }

    public void RenderContent() { } // no content
}