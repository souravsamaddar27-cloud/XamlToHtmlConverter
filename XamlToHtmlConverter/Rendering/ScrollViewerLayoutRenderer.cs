using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

public class ScrollViewerLayoutRenderer : ILayoutRenderer
{
    public int Priority => 90;

    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "ScrollViewer";

    public void ApplyLayout(IntermediateRepresentationElement element, StringBuilder sb)
        {
            var hVis = element.Properties.GetValueOrDefault("HorizontalScrollBarVisibility", "Auto");
            var vVis = element.Properties.GetValueOrDefault("VerticalScrollBarVisibility", "Auto");

            sb.Append($"overflow-x:{MapScrollVisibility(hVis)};");
            sb.Append($"overflow-y:{MapScrollVisibility(vVis)};");
        }

        private static string MapScrollVisibility(string value) => value switch
        {
            "Disabled" => "hidden",
            "Hidden" => "hidden",
            "Visible" => "scroll",
            _ => "auto"
        };
}