using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Xml.Linq;
using XamlToHtmlConverter.IR;

namespace XamlToHtmlConverter.Parsing
{
    /// <summary>
    /// Converts XML elements into IR elements using
    /// a structured recursive processing approach.
    /// Separates attribute, text, and child handling into dedicated methods.
    /// </summary>
    public class XmlToIrConverterRecursive : IXmlToIrConverter
    {
        /// <summary>
        /// Validates input and starts recursive conversion.
        /// </summary>
        public IrElement Convert(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var root = ConvertElement(element);

            // ?? Second pass: resolve StaticResource
            ResolveStaticResources(root);

            return root;
        }
        /// <summary>
        /// Converts a single XElement into an IrElement
        /// and delegates processing steps.
        /// </summary>
        private IrElement ConvertElement(XElement element)
        {
            var ir = new IrElement(element.Name.LocalName);
            ProcessAttributes(element, ir);
            ProcessText(element, ir);
            ProcessChildren(element, ir);
            

            return ir;
        }

        /// <summary>
        /// Maps XML attributes to standard or attached IR properties.
        /// </summary>
        private void ProcessAttributes(XElement element, IrElement ir)
        {
            foreach(var attr in element.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                    continue;

                var name = attr.Name.LocalName;
                var value = attr.Value;

                if (IsAttachedProperty(name))
                    ir.AttachedProperties[name] = value;
                else
                    ir.Properties[name] = value;
            }
        }

        /// <summary>
        /// Extracts and assigns combined non-empty text nodes
        /// as the IR element's inner text.
        /// </summary>
        private void ProcessText(XElement element,IrElement ir)
        {
            var text = string.Join(" ",element.Nodes().OfType<XText>().Select(t=>t.Value.Trim()).Where(t=>!string.IsNullOrWhiteSpace(t)));
            if (!string.IsNullOrWhiteSpace(text))
                ir.InnerText = text;
        }

        /// <summary>
        /// Recursively converts child XML elements
        /// and adds them to the IR children collection.
        /// </summary>

        private void ProcessChildren(XElement element, IrElement ir)
        {
            foreach (var child in element.Elements())
            {
                
                var childName = child.Name.LocalName;

                // 1?? Handle Resources (NEW)
                //if (child.Name.LocalName.EndsWith(".Resources"))
                //{
                //    ParseResources(child, ir);
                //    continue;
                //}
                if (child.Name.LocalName.EndsWith(".Resources"))
                {
                    ParseResources(child, ir);
                    continue;
                }

                // Handle Template property elements
                if (childName.EndsWith(".Template"))
                {
                    var templateElement = child.Elements().FirstOrDefault();
                    if (templateElement != null)
                    {
                        var templateIr = ConvertElement(templateElement);
                        templateIr.Parent = ir;
                        ir.Template = templateIr;
                    }

                    continue;
                }

                // 2?? Handle Grid.RowDefinitions
                if (childName == "Grid.RowDefinitions")
                {
                    foreach (var rowDef in child.Elements())
                    {
                        var heightAttr = rowDef.Attribute("Height");
                        if (heightAttr != null)
                        {
                            ir.GridRowDefinitions.Add(heightAttr.Value);
                        }
                    }
                    continue;
                }

                // 3?? Handle Grid.ColumnDefinitions
                if (childName == "Grid.ColumnDefinitions")
                {
                    foreach (var colDef in child.Elements())
                    {
                        var widthAttr = colDef.Attribute("Width");
                        if (widthAttr != null)
                        {
                            ir.GridColumnDefinitions.Add(widthAttr.Value);
                        }
                    }
                    continue;
                }

                // 4?? Normal children
                var childIr = ConvertElement(child);
                childIr.Parent = ir;   // IMPORTANT for resource lookup
                ir.Children.Add(childIr);
            }
        }

        /// <summary>
        /// Determines whether an attribute represents
        /// an attached property based on naming convention.
        /// </summary>
        private bool IsAttachedProperty(string name) {
            return name.Contains(".");
        }

        private void ParseResources(XElement resourcesNode, IrElement ir)
        {
            foreach (var style in resourcesNode.Elements())
            {
                if (style.Name.LocalName != "Style")
                    continue;

                var keyAttr = style.Attributes()
                                   .FirstOrDefault(a => a.Name.LocalName == "Key");

                var targetTypeAttr = style.Attribute("TargetType");
                var basedOnAttr = style.Attribute("BasedOn");

                var styleObject = new IrStyle();

                if (keyAttr != null)
                {
                    styleObject.Key = keyAttr.Value;
                }

                if (targetTypeAttr != null)
                {
                    styleObject.TargetType = targetTypeAttr.Value;
                }
                if (basedOnAttr != null)
                {
                    styleObject.BasedOn = ExtractStaticResourceKey(basedOnAttr.Value);
                }

                foreach (var setter in style.Elements()
                                             .Where(e => e.Name.LocalName == "Setter"))
                {
                    var propAttr = setter.Attribute("Property");
                    var valueAttr = setter.Attribute("Value");

                    if (propAttr != null && valueAttr != null)
                    {
                        styleObject.Setters[propAttr.Value] = valueAttr.Value;
                    }
                }

                // Store style
                if (styleObject.Key != null)
                {
                    ir.Resources[styleObject.Key] = styleObject;
                }
                else if (styleObject.TargetType != null)
                {
                    ir.Resources["__implicit__" + styleObject.TargetType] = styleObject;
                }
            }
        }
        private void ApplyStaticResource(IrElement element)
        {
            if (!element.Properties.TryGetValue("Style", out var styleValue))
                return;

            if (!styleValue.StartsWith("{StaticResource"))
                return;

            var key = styleValue
                .Replace("{StaticResource", "")
                .Replace("}", "")
                .Trim();

            var style = FindResource(element.Parent, key);
            if (style == null)
                return;

            ApplyStyleWithInheritance(element, style);
        }
        private void ApplyImplicitStyle(IrElement element)
        {
            var implicitKey = "__implicit__" + element.Type;

            var style = FindResource(element.Parent, implicitKey);

            if (style == null)
                return;

            foreach (var setter in style.Setters)
            {
                // Do NOT override explicitly set properties
                if (!element.Properties.ContainsKey(setter.Key))
                    element.Properties[setter.Key] = setter.Value;
            }
        }

        private void ApplyStyleWithInheritance(IrElement element, IrStyle style)
        {
            // 1️⃣ Apply parent style first
            if (!string.IsNullOrWhiteSpace(style.BasedOn))
            {
                var parentStyle = FindResource(element.Parent, style.BasedOn);
                if (parentStyle != null)
                {
                    ApplyStyleWithInheritance(element, parentStyle);
                }
            }

            // 2️⃣ Apply this style's setters
            foreach (var setter in style.Setters)
            {
                if (!element.Properties.ContainsKey(setter.Key))
                    element.Properties[setter.Key] = setter.Value;
            }
        }
        private IrStyle? FindResource(IrElement? element, string key)
        {
            while (element != null)
            {
                if (element.Resources.TryGetValue(key, out var style))
                    return style;

                element = element.Parent;
            }

            return null;
        }
        private void ResolveStaticResources(IrElement element)
        {
            // 1️⃣ Apply implicit style
            ApplyImplicitStyle(element);

            // 2️⃣ Apply explicit StaticResource
            ApplyStaticResource(element);

            // 3️⃣ Propagate DataContext to children
            foreach (var child in element.Children)
            {
                PropagateDataContext(element, child);
                ResolveStaticResources(child);
            }
        }
        private void PropagateDataContext(IrElement parent, IrElement child)
        {
            // If child already has DataContext, do nothing
            if (child.Properties.ContainsKey("DataContext"))
                return;

            // If parent has DataContext, inherit it
            if (parent.Properties.TryGetValue("DataContext", out var dataContext))
            {
                child.Properties["DataContext"] = dataContext;
            }
        }
        private string? ExtractStaticResourceKey(string value)
        {
            if (!value.StartsWith("{StaticResource"))
                return null;

            return value
                .Replace("{StaticResource", "")
                .Replace("}", "")
                .Trim();
        }
    }
}