using System;
using System.Collections.Generic;
using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Manages reusable CSS styles by mapping inline style strings
    /// to generated CSS class names and producing a consolidated style block.
    /// </summary>
    public class StyleRegistry
    {
        /// <summary>
        /// Maps style strings to generated CSS class names.
        /// </summary>
        private readonly Dictionary<string, string> _styleToClass = new();

        /// <summary>
        /// Maps generated class names back to their style definitions.
        /// </summary>
        private readonly Dictionary<string, string> _classToStyle = new();
        private int _counter = 1;

        /// <summary>
        /// Registers a style string and returns a corresponding CSS class name.
        /// Reuses existing class names for duplicate styles.
        /// </summary>
        public string Register(string style)
        {
            if (string.IsNullOrWhiteSpace(style))
                return string.Empty;

            if (_styleToClass.TryGetValue(style, out var existing))
                return existing;

            var className = $"c{_counter++}";
            _styleToClass[style] = className;
            _classToStyle[className] = style;

            return className;
        }
        /// <summary>
        /// Generates a complete HTML style block
        /// containing all registered CSS class definitions.
        /// </summary>
        public string GenerateStyleBlock()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<style>");

            foreach (var kvp in _classToStyle)
            {
                sb.AppendLine($".{kvp.Key} {{ {kvp.Value} }}");
            }

            sb.AppendLine("</style>");
            return sb.ToString();
        }
    }
}
