using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

public class ListBoxRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
    {
        return element.Type.EndsWith("ListBox", StringComparison.OrdinalIgnoreCase);
    }

    public void RenderAttributes(
    IntermediateRepresentationElement element,
    AttributeBuffer attributes)
    {
        attributes.Add("multiple", string.Empty);
    }

    public void RenderContent(
    IntermediateRepresentationElement element,
    StringBuilder sb,
    int indent,
    Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        Console.WriteLine("ItemTemplate is null: " + (element.ItemTemplate == null));
        if (element.ItemTemplate == null)
            return;

        var indentation = new string(' ', indent + 2);

        // Detect ItemsSource binding
        if (element.Bindings.TryGetValue("ItemsSource", out var binding))
        {
            sb.AppendLine();
            sb.AppendLine($"{indentation}<!-- ItemsSource: {binding.Path} -->");

            // Render placeholder items
            for (int i = 0; i < 3; i++)
            {
                sb.AppendLine($"{indentation}<option data-item-index=\"{i}\">");

                renderChild(element.ItemTemplate, sb, indent + 4);

                sb.AppendLine($"{indentation}</option>");
            }

            return;
        }

        // Fallback: single template
        sb.AppendLine();
        sb.AppendLine($"{indentation}<option>");

        renderChild(element.ItemTemplate, sb, indent + 4);

        sb.AppendLine($"{indentation}</option>");
    }
}