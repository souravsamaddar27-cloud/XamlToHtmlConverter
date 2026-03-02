using System;
using System.IO;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.IR;
using XamlToHtmlConverter.Rendering;

/// <summary>
/// Entry point of the application.
/// Coordinates XAML loading, IR conversion, XML export,
/// HTML rendering, and console inspection.
/// </summary>
class Program
{
    /// <summary>
    /// Executes the end-to-end XAML to HTML conversion pipeline.
    /// Handles loading, transformation, exporting, and output generation.
    /// </summary>
    static void Main()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "sample.xaml");

        var loader = new XamlLoader();
        var document = loader.Load(path);
        if (document.Root == null)
            throw new InvalidOperationException("XML document has no root element.");

        // Save original XML DOM
        var xmlOutputPath = Path.Combine(AppContext.BaseDirectory, "XamlDom.xml");
        document.Save(xmlOutputPath);

        // Select conversion strategy
        Console.WriteLine("Recursive converter running...");
        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        //Console.WriteLine("LINQ converter running...");
        //IXmlToIrConverter converter = new XmlToIrConverterLinqStyle();
        var ir = converter.Convert(document.Root);


        // Save IR representation
        var irDoc = IrXmlExporter.Export(ir);
        var irOutputPath = Path.Combine(AppContext.BaseDirectory, "Ir.xml");
        irDoc.Save(irOutputPath);

        var renderer = new HtmlRenderer(
            new DefaultElementTagMapper(), 
            new ILayoutRenderer[]
            {
                new GridLayoutRenderer(),
                new StackPanelLayoutRenderer(),
                new DockPanelLayoutRenderer()
            },
            new DefaultStyleBuilder(),
            new DefaultEventExtractor());
        var html = renderer.RenderDocument(ir);
        var htmlOutputPath = Path.Combine(AppContext.BaseDirectory, "output.html");
        File.WriteAllText(htmlOutputPath, html);

        // Print IR structure to console
        PrintIr(ir, 0);
        Console.ReadLine();
    }

    /// <summary>
    /// Recursively prints the IR tree structure,
    /// including properties, attached properties, and text.
    /// </summary>
    static void PrintIr(IrElement element, int indent)
    {
        var space = new string(' ', indent);
        Console.WriteLine($"{space}{element.Type}");

        foreach (var prop in element.Properties)
            Console.WriteLine($"{space}  prop: {prop.Key}={prop.Value}");

        foreach (var attached in element.AttachedProperties)
            Console.WriteLine($"{space}  Attached: {attached.Key}={attached.Value}");

        if (!string.IsNullOrWhiteSpace(element.InnerText))
            Console.WriteLine($"{space}  Text: {element.InnerText}");

        // ✅ Print Template separately (NEW)
        if (element.Template != null)
        {
            Console.WriteLine($"{space}  [Template]");
            PrintIr(element.Template, indent + 4);
        }

        foreach (var child in element.Children)
            PrintIr(child, indent + 2);
    }
}