using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

/// <summary>
/// Routes element properties to their corresponding CSS style mappers.
/// Implements IPropertyMapperEngine to satisfy the Dependency Inversion Principle.
/// </summary>
public class PropertyMapperEngine : IPropertyMapperEngine
{
    private readonly IEnumerable<IPropertyMapper> v_Mappers;

    /// <summary>
    /// Initializes the engine with a collection of property mappers.
    /// </summary>
    /// <param name="mappers">The available property mappers.</param>
    public PropertyMapperEngine(IEnumerable<IPropertyMapper> mappers)
    {
        v_Mappers = mappers;
    }

    /// <summary>
    /// Applies configured property mappers to an element's properties,
    /// converting them to CSS styles.
    /// </summary>
    public void Apply(
        IntermediateRepresentationElement element,
        LayoutContext context,
        StringBuilder styleBuilder)
    {
        foreach (var prop in element.Properties)
        {
            foreach (var mapper in v_Mappers)
            {
                if (mapper.CanHandle(prop.Key))
                {
                    mapper.Apply(element, prop.Key, context, styleBuilder);
                    break;
                }
            }
        }
    }
}