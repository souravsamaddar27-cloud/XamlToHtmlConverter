using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Layout renderer responsible for handling DockPanel elements.
    /// Converts DockPanel behavior into a flexbox-based layout.
    /// </summary>
    public class DockPanelLayoutRenderer : ILayoutRenderer
    {
        /// <summary>
        /// Determines whether this renderer can handle the specified IR element.
        /// </summary>
        public bool CanHandle(IrElement element)
            => element.Type == "DockPanel";

        public void ApplyLayout(IrElement element, StringBuilder sb)
        {
            /// <summary>
            /// Applies flexbox layout rules for DockPanel.
            /// Uses column direction if any child is docked Top/Bottom;
            /// otherwise defaults to horizontal (row) layout.
            /// </summary>
            sb.Append("display:flex;");

            // For MVP: if any child docks Top/Bottom,
            // use column direction.
            foreach (var child in element.Children)
            {
                if (child.AttachedProperties.TryGetValue("DockPanel.Dock", out var dock))
                {
                    if (dock == "Top" || dock == "Bottom")
                    {
                        sb.Append("flex-direction:column;");
                        return;
                    }
                }
            }

            // Default horizontal layout
            sb.Append("flex-direction:row;");
        }
    }
}