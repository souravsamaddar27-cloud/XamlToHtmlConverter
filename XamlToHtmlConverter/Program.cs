using System;
using System.IO;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.IR;

class Program
{
    static void Main()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "sample.xaml");

        var loader = new XamlLoader();
        var document = loader.Load(path);
        if (document.Root == null)
            throw new InvalidOperationException("XML document has no root element.");

        //Switch Strategy here
       // IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        IXmlToIrConverter converter = new XmlToIrConverterLinqStyle();
        var ir = converter.Convert(document.Root);
        PrintIr(ir, 0);
        Console.ReadLine();
    }
    static void PrintIr(IrElement element, int indent)
    {
        var space = new string(' ', indent);
        Console.WriteLine($"{space} {element.Type}");

        foreach (var prop in element.Properties)
            Console.WriteLine($"{space} prop: {prop.Key}={prop.Value}");

        foreach (var attached in element.AttachedProperties)
            Console.WriteLine($"{space} Attached: {attached.Key}={attached.Value}");

        if (!string.IsNullOrWhiteSpace(element.InnerText))
            Console.WriteLine($"{space} Text: {element.InnerText}");

        foreach (var child in element.Children)
            PrintIr(child, indent + 2);

    }
}