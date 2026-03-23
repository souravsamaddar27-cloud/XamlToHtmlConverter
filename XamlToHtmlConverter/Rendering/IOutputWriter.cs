// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Defines the interface for output writing operations.
/// Centralizes all file I/O operations (XML serialization, text file writing)
/// to satisfy the Single Responsibility Principle and enable dependency inversion.
/// 
/// This allows ConversionPipeline to focus purely on orchestration,
/// while delegating output concerns to a dedicated service.
/// </summary>
public interface IOutputWriter
{
    /// <summary>
    /// Writes an XML document to the specified file path.
    /// </summary>
    /// <param name="document">The XML document to write.</param>
    /// <param name="filePath">The target file path.</param>
    /// <exception cref="IOException">Thrown if the file cannot be written.</exception>
    void WriteXmlDocument(XDocument document, string filePath);

    /// <summary>
    /// Writes HTML content to the specified file path with UTF-8 encoding.
    /// </summary>
    /// <param name="content">The HTML content to write.</param>
    /// <param name="filePath">The target file path.</param>
    /// <exception cref="ArgumentException">Thrown if content is null.</exception>
    /// <exception cref="IOException">Thrown if the file cannot be written.</exception>
    void WriteHtmlContent(string content, string filePath);
}
