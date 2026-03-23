// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

/// <summary>
/// Defines the interface for property mapping engines that apply
/// element properties to CSS styles via configured mappers.
/// Implements the Dependency Inversion Principle - clients depend on this
/// abstraction rather than concrete PropertyMapperEngine implementations.
/// </summary>
public interface IPropertyMapperEngine
{
    /// <summary>
    /// Applies configured property mappers to an element's properties,
    /// converting them to CSS styles in the provided style builder.
    /// </summary>
    /// <param name="element">The IR element whose properties should be mapped.</param>
    /// <param name="context">The layout context providing parent/sibling information.</param>
    /// <param name="styleBuilder">The style builder to accumulate CSS declarations.</param>
    void Apply(
        IntermediateRepresentationElement element,
        LayoutContext context,
        StringBuilder styleBuilder);
}
