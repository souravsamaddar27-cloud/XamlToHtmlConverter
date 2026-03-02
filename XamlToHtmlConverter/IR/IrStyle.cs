using System;
using System.Collections.Generic;
using System.Text;

namespace XamlToHtmlConverter.IR
{
    /// <summary>
    /// Represents a style definition in the IR layer.
    /// Stores metadata and property setters extracted from XAML.
    /// </summary>
    public class IrStyle
    {
        /// <summary>
        /// Gets or sets the unique key identifying the style.
        /// </summary>
        public string? Key { get; set; }
        /// <summary>
        /// Gets or sets the target element type
        /// to which the style applies.
        /// </summary>
        public string? TargetType { get; set; }
        /// <summary>
        /// Gets or sets the base style reference
        /// for style inheritance scenarios.
        /// </summary>
        public string? BasedOn { get; set; }
        /// <summary>
        /// Collection of property-value pairs
        /// defined as setters within the style.
        /// </summary>
        public Dictionary<string, string> Setters { get; } = new();
    }
}
