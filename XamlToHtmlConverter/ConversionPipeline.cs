// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Diagnostics;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter;

/// <summary>
/// Orchestrates the end-to-end XAML-to-HTML conversion pipeline.
/// Extracted from Program.Main() to satisfy Single Responsibility Principle.
/// 
/// Responsibilities:
///   - XAML document loading
///   - Intermediate Representation (IR) construction
///   - HTML rendering
/// 
/// All file I/O operations are delegated to IOutputWriter to maintain
/// separation of concerns (orchestration vs. I/O).
/// Performance metrics are collected during execution for observability.
/// </summary>
public class ConversionPipeline
{
    private readonly IXmlToIrConverter v_Converter;
    private readonly HtmlRenderer v_Renderer;
    private readonly IOutputWriter v_OutputWriter;

    /// <summary>
    /// Initializes a new ConversionPipeline with the specified converter, renderer, and output writer.
    /// </summary>
    /// <param name="converter">The converter strategy for transforming XML to IR.</param>
    /// <param name="renderer">The renderer for transforming IR to HTML.</param>
    /// <param name="outputWriter">The output writer for persisting files (default: OutputWriter).</param>
    public ConversionPipeline(IXmlToIrConverter converter, HtmlRenderer renderer, IOutputWriter? outputWriter = null)
    {
        v_Converter = converter;
        v_Renderer = renderer;
        v_OutputWriter = outputWriter ?? new OutputWriter();
    }

    /// <summary>
    /// Executes the conversion pipeline on the specified input file,
    /// writing outputs to the specified directory.
    /// </summary>
    /// <param name="inputPath">The path to the input XAML file.</param>
    /// <param name="outputDirectory">The directory where output files will be written.</param>
    /// <returns>Performance metrics collected during conversion.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the input file is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the XML document has no root element.</exception>
    public ConversionMetrics Run(string inputPath, string outputDirectory)
    {
        var totalWatch = Stopwatch.StartNew();

        // Phase 1: Load XAML
        var loadWatch = Stopwatch.StartNew();
        var loader = new XamlLoader();
        var document = loader.Load(inputPath);
        loadWatch.Stop();

        if (document.Root == null)
            throw new InvalidOperationException("XML document has no root element.");

        // Phase 2: Convert to IR
        var conversionWatch = Stopwatch.StartNew();
        var ir = v_Converter.Convert(document.Root);
        conversionWatch.Stop();

        // Count elements for metrics
        var elementCount = CountElements(ir);

        // Phase 3: Render HTML
        var renderingWatch = Stopwatch.StartNew();
        var html = v_Renderer.RenderDocument(ir);
        renderingWatch.Stop();

        // Phase 4: Write outputs (delegated to IOutputWriter)
        var writeWatch = Stopwatch.StartNew();
        var xmlOutputPath = Path.Combine(outputDirectory, "XamlDom.xml");
        v_OutputWriter.WriteXmlDocument(document, xmlOutputPath);

        var irDoc = IntermediateRepresentationXmlExporter.Export(ir);
        var irOutputPath = Path.Combine(outputDirectory, "Ir.xml");
        v_OutputWriter.WriteXmlDocument(irDoc, irOutputPath);

        var htmlOutputPath = Path.Combine(outputDirectory, "output.html");
        v_OutputWriter.WriteHtmlContent(html, htmlOutputPath);
        writeWatch.Stop();

        totalWatch.Stop();

        totalWatch.Stop();

        // Collect and return metrics
        return new ConversionMetrics
        {
            LoadingTime = loadWatch.Elapsed,
            ConversionTime = conversionWatch.Elapsed,
            RenderingTime = renderingWatch.Elapsed,
            TotalTime = totalWatch.Elapsed,
            ElementCount = elementCount,
            StyleCount = CountStyles(html),
            InputFilePath = inputPath,
            OutputDirectory = outputDirectory
        };
    }

    /// <summary>
    /// Recursively counts all elements in the IR tree.
    /// </summary>
    private static int CountElements(IntermediateRepresentationElement element)
    {
        int count = 1;
        foreach (var child in element.Children)
            count += CountElements(child);
        return count;
    }

    /// <summary>
    /// Counts CSS classes in the rendered HTML output.
    /// </summary>
    private static int CountStyles(string html)
    {
        return html.Split("class=").Length - 1;
    }
}

/// <summary>
/// Represents performance metrics collected during XAML-to-HTML conversion.
/// </summary>
public class ConversionMetrics
{
    /// <summary>
    /// Time spent loading the XAML file from disk.
    /// </summary>
    public TimeSpan LoadingTime { get; set; }

    /// <summary>
    /// Time spent converting XML to IR representation.
    /// </summary>
    public TimeSpan ConversionTime { get; set; }

    /// <summary>
    /// Time spent rendering IR to HTML string.
    /// </summary>
    public TimeSpan RenderingTime { get; set; }

    /// <summary>
    /// Total end-to-end conversion time.
    /// </summary>
    public TimeSpan TotalTime { get; set; }

    /// <summary>
    /// Total number of elements in the IR tree.
    /// </summary>
    public int ElementCount { get; set; }

    /// <summary>
    /// Number of CSS classes generated during rendering.
    /// </summary>
    public int StyleCount { get; set; }

    /// <summary>
    /// The input XAML file path.
    /// </summary>
    public string? InputFilePath { get; set; }

    /// <summary>
    /// The output directory where HTML and IR XML are written.
    /// </summary>
    public string? OutputDirectory { get; set; }

    /// <summary>
    /// Formats metrics as a human-readable string.
    /// </summary>
    public override string ToString()
    {
        return $"╔════════════════════════════════════════════╗\n" +
               $"║      XAML-to-HTML Conversion Metrics      ║\n" +
               $"╚════════════════════════════════════════════╝\n\n" +
               $"Input File         : {Path.GetFileName(InputFilePath)}\n" +
               $"Output Directory   : {OutputDirectory}\n\n" +
               $"═══ EXECUTION TIME ═══════════════════════════\n" +
               $"Loading Time       : {LoadingTime.TotalMilliseconds:F2} ms\n" +
               $"Conversion Time    : {ConversionTime.TotalMilliseconds:F2} ms\n" +
               $"Rendering Time     : {RenderingTime.TotalMilliseconds:F2} ms\n" +
               $"─────────────────────────────────────────\n" +
               $"Total Time         : {TotalTime.TotalMilliseconds:F2} ms\n\n" +
               $"═══ DOCUMENT METRICS ═════════════════════════\n" +
               $"Element Count      : {ElementCount:N0}\n" +
               $"CSS Classes        : {StyleCount}\n" +
               $"ms/Element         : {(TotalTime.TotalMilliseconds / ElementCount):F4}\n";
    }
}
