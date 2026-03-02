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
        /// <summary>
        /// Collection of row definitions used when the element represents a Grid.
        /// Stores raw XAML GridLength values (e.g., "Auto", "*", "2*").
        /// </summary>
        public List<string> GridRowDefinitions { get; } = new();
        /// <summary>
        /// Collection of column definitions used when the element represents a Grid.
        /// Stores raw XAML GridLength values for column sizing.
        /// </summary>
        public List<string> GridColumnDefinitions { get; } = new();
        /// <summary>
        /// Reference to the parent IR element in the visual tree.
        /// Enables upward traversal and context-aware processing.
        /// </summary>
        public IrElement? Parent { get; set; }
        /// <summary>
        /// Resource dictionary associated with this element.
        /// Stores keyed style definitions and other reusable resources.
        /// </summary>
        public Dictionary<string, IrStyle> Resources { get; } = new();
        /// <summary>
        /// Represents the control template associated with this element,
        /// if one is defined.
        /// </summary>
        public IrElement? Template { get; set; }

        public IrElement(string type)
        {
            Type = type;
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            AttachedProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Children = new List<IrElement>();
        }
    }
}
