// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for MediaElement, mapping to HTML video or audio depending on source file type.
/// </summary>
public class MediaElementRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "MediaElement";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // Media source is handled via attributes, not content
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Determine whether to use video or audio based on source extension
        string mediaType = DetermineMediaType(element);

        // Handle Source property
        if (element.Properties.TryGetValue("Source", out var source))
        {
            attributes.Add("src", source);
        }

        // Handle AutoPlay property
        if (element.Properties.TryGetValue("AutoPlay", out var autoPlay)
            && autoPlay.Equals("True", StringComparison.OrdinalIgnoreCase))
            attributes.Add("autoplay", "");

        // Handle IsMuted property
        if (element.Properties.TryGetValue("IsMuted", out var isMuted)
            && isMuted.Equals("True", StringComparison.OrdinalIgnoreCase))
            attributes.Add("muted", "");

        // Handle LoadedBehavior for controls display
        if (element.Properties.TryGetValue("LoadedBehavior", out var loadedBehavior))
        {
            if (loadedBehavior.Equals("Play", StringComparison.OrdinalIgnoreCase))
                attributes.Add("autoplay", "");
            else if (loadedBehavior.Equals("Manual", StringComparison.OrdinalIgnoreCase))
                attributes.Add("controls", "");
        }
        else
        {
            // Default to showing controls
            attributes.Add("controls", "");
        }

        // Handle bindings
        if (element.Bindings.TryGetValue("Source", out var binding)
            && !string.IsNullOrEmpty(binding?.Path))
            attributes.Add("data-binding-source", binding.Path!);

        // Store media type info for marker attributes (since HtmlRenderer won't know the derived tag)
        attributes.Add("data-media-type", mediaType);
    }

    /// <summary>
    /// Determines whether this is a video or audio media element based on source file extension.
    /// </summary>
    private string DetermineMediaType(IntermediateRepresentationElement element)
    {
        if (!element.Properties.TryGetValue("Source", out var source))
            return "video"; // Default to video if no source specified

        // Check file extension to determine type
        var extension = Path.GetExtension(source).ToLowerInvariant();

        return extension switch
        {
            ".mp3" or ".wav" or ".aac" or ".ogg" or ".flac" => "audio",
            ".mp4" or ".webm" or ".mkv" or ".ogv" or ".mov" => "video",
            _ => "video" // Default to video for unknown types
        };
    }
}
