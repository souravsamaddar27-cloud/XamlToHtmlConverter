using System.Collections.Generic;

namespace XamlToHtmlConverter.IR
{
    /// <summary>
    /// Represents a neutral intermediate representation (IR) element.
    /// This model is independent from WPF runtime types.
    /// </summary>
    public class IrElement
    {
        /// <summary>
        /// XAML element type name (e.g., Grid, Button, TextBlock).
        /// </summary>
        public string Type { get; }

       

        /// <summary>
        /// Regular properties defined as attributes in XAML.
        /// Example: Width="100"
        /// </summary>
        public Dictionary<string, string> Properties { get; }

        /// <summary>
        /// Direct inner text content of the element (if any).
        /// </summary>
        public string? InnerText { get; set; }

        /// <summary>
        /// Attached properties (e.g., Grid.Row="1").
        /// These are stored separately for layout mapping.
        /// </summary>
        public Dictionary<string, string> AttachedProperties { get; }

        /// <summary>
        /// Child elements in the logical tree.
        /// </summary>
        public List<IrElement> Children { get; set; }

        public List<string> GridRowDefinitions { get; } = new();
        public List<string> GridColumnDefinitions { get; } = new();

        public IrElement(string type)
        {
            Type = type;
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            AttachedProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Children = new List<IrElement>();
        }
    }
}
