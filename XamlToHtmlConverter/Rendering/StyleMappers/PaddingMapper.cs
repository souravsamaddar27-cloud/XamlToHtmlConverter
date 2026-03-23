using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class PaddingMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "Padding";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        var value = element.Properties[propertyName];

        if (string.IsNullOrWhiteSpace(value))
            return;

        var parts = value.Split(',');

        if (parts.Length == 1 && int.TryParse(parts[0], out var all))
        {
            sb.Append($"padding:{all}px;");
        }
        else if (parts.Length == 2)
        {
            if (int.TryParse(parts[0], out var horizontal) &&
                int.TryParse(parts[1], out var vertical))
            {
                // CSS expects vertical first, then horizontal
                sb.Append($"padding:{vertical}px {horizontal}px;");
            }
        }
        else if (parts.Length == 4)
        {
            if (int.TryParse(parts[0], out var left) &&
                int.TryParse(parts[1], out var top) &&
                int.TryParse(parts[2], out var right) &&
                int.TryParse(parts[3], out var bottom))
            {
                sb.Append($"padding:{top}px {right}px {bottom}px {left}px;");
            }
        }
    }
}