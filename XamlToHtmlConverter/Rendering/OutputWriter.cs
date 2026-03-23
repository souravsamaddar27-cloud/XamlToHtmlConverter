// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Concrete implementation of IOutputWriter that handles file I/O operations.
/// Centralizes XML serialization and text file writing, keeping ConversionPipeline
/// focused purely on orchestration logic.
/// 
/// Thread-safe: All operations are atomic filesystem operations.
/// </summary>
public class OutputWriter : IOutputWriter
{
    /// <summary>
    /// Writes an XML document to the specified file path.
    /// Creates parent directories if they don't exist.
    /// </summary>
    /// <param name="document">The XML document to write.</param>
    /// <param name="filePath">The target file path.</param>
    /// <exception cref="IOException">Thrown if the file cannot be written.</exception>
    public void WriteXmlDocument(XDocument document, string filePath)
    {
        try
        {
            // Ensure parent directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            document.Save(filePath);
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to write XML document to '{filePath}'.", ex);
        }
    }

    /// <summary>
    /// Writes HTML content to the specified file path with UTF-8 encoding.
    /// Creates parent directories if they don't exist.
    /// </summary>
    /// <param name="content">The HTML content to write.</param>
    /// <param name="filePath">The target file path.</param>
    /// <exception cref="ArgumentException">Thrown if content is null.</exception>
    /// <exception cref="IOException">Thrown if the file cannot be written.</exception>
    public void WriteHtmlContent(string content, string filePath)
    {
        if (content == null)
            throw new ArgumentException("Content cannot be null.", nameof(content));

        try
        {
            // Ensure parent directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, content, System.Text.Encoding.UTF8);
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to write HTML content to '{filePath}'.", ex);
        }
    }
}
