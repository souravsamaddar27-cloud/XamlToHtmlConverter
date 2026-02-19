using System;
using XamlToHtmlConverter.IR;
using System.IO;
using XamlToHtmlConverter.Parsing;



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