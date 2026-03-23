// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Defines a contract for building inline CSS style strings
    /// based on element properties and parent layout context.
    /// </summary>
    public interface IStyleBuilder
    {
        /// <summary>Registers a CSS style string; returns a reusable class name.
        ///</summary>
        string Register(string cssStyle);
        /// <summary>Returns the full <style> block for the HTML <head>.</summary>
        string GenerateStyleBlock();
        /// <summary>
        /// Generates a CSS style string for the specified IR element,
        /// taking into account its parent layout context for alignment and positioning.
        /// </summary>
        /// <param name="element">The IR element to generate styles for.</param>
        /// <param name="context">The layout context describing the parent container type and orientation.</param>
        /// <returns>A CSS style string (e.g., "width:100px;height:50px;").</returns>
        string Build(IntermediateRepresentationElement element, LayoutContext context);

        /// <summary>
        /// Extracts data binding attributes from the IR element's properties
        /// and returns them as HTML-compatible data-binding-* key-value pairs.
        /// </summary>
        /// <param name="element">The IR element to scan for binding expressions.</param>
        /// <returns>A dictionary of HTML attribute names to binding path strings.</returns>
        Dictionary<string, string> ExtractBindingAttributes(IntermediateRepresentationElement element);
    }
}
