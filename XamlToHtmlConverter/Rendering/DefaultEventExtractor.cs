using System;
using System.Collections.Generic;
using System.Text;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Default implementation of IEventExtractor.
    /// Extracts known XAML event handlers and converts them
    /// into HTML data-event attributes.
    /// </summary>
    public class DefaultEventExtractor : IEventExtractor
        {
        /// <summary>
        /// Defines the set of supported XAML event names
        /// that can be converted into HTML metadata attributes.
        /// </summary>
        private static readonly HashSet<string> KnownEvents = new()
            {
                "Click",
                "TextChanged",
                "Checked",
                "Unchecked",
                "Loaded",
                "SelectionChanged"
            };
        /// <summary>
        /// Scans element properties for known events
        /// and returns corresponding data-event-* attributes.
        /// </summary>
        public Dictionary<string, string> Extract(IrElement element)
            {
                var result = new Dictionary<string, string>();

                foreach (var prop in element.Properties)
                {
                    if (KnownEvents.Contains(prop.Key))
                    {
                        result[$"data-event-{prop.Key.ToLower()}"] = prop.Value;
                    }
                }

                return result;
            }
        }
}
