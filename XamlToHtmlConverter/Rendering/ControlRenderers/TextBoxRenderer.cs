using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

public class TextBoxRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "TextBox";

    public void RenderAttributes(
        IntermediateRepresentationElement element,
        AttributeBuffer attributes)
    {
        bool acceptsReturn = element.Properties.TryGetValue("AcceptsReturn", out var ar)
            && ar.Equals("True", StringComparison.OrdinalIgnoreCase);

        if (!acceptsReturn)
            attributes.Add("type", "text");

        if (element.Properties.TryGetValue("Text", out var text))
            attributes.Add("value", text);

        if (element.Properties.TryGetValue("MaxLength", out var max))
            attributes.Add("maxlength", max);

        if (element.Properties.TryGetValue("IsReadOnly", out var ro)
            && ro.Equals("True", StringComparison.OrdinalIgnoreCase))
            attributes.Add("readonly", "");
    }

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // TextBox has no template or child rendering
    }
}
