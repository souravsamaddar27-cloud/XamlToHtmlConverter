using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class VisibilityMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName) => propertyName == "Visibility";

    public void Apply(IntermediateRepresentationElement element,
        string propertyName, LayoutContext context, StringBuilder sb)
    {
        var value = element.Properties[propertyName];

        if (!value.StartsWith("{Binding"))
        {
            sb.Append(value.Equals("Collapsed", StringComparison.OrdinalIgnoreCase)
                ? "display:none;"
                : "visibility:hidden;");
        }
    }
}