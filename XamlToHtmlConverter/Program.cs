using System;
using XamlToHtmlConverter.IR;
using System.IO;
using XamlToHtmlConverter.Parsing;


//class program
//{
//    static void Main(string[] args)
//    {
//        var parser = new XamlParser();
//        var ir = parser.Parse("sample.xaml");
//        printIr(ir, 0);
//    }

//    static void printIr(IrElement element, int indent)
//    {
//        Console.WriteLine($"{new string(' ', indent)}{element.Type}");

//        foreach (var prop in element.Properties)
//            Console.WriteLine($"{new string(' ', indent + 2)}Prop: {prop.Key}={prop.Value}");
//        foreach (var attached in element.AttachedProperties)
//            Console.WriteLine($"{new string(' ', indent + 2)}Attached: {attached.Key}={attached.Value}");
//        foreach (var child in element.Children)
//            printIr(child, indent + 2);
//    }
//}

class Program
{
    static void Main()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "sample.xaml");
        var loader = new XamlLoader();
        var document = loader.Load(path);
        if (document.Root == null)
             throw new InvalidOperationException("XML document has no root element.");

        Console.WriteLine("=== XML DOM Structure ===");
        XmlDomInspector.Print(document.Root);
        //Console.ReadLine();
    }
}