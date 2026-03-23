// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Default implementation of <see cref="IElementTagMapper"/>.
/// Maps known XAML element type names to their corresponding HTML tags.
/// Supports extensibility through constructor-injected overrides.
/// Falls back to 'div' when no specific mapping is registered.
/// 
/// Satisfies Open/Closed Principle:
///   - Closed for modification: Built-in mappings are static and immutable
///   - Open for extension: Callers can provide custom mappings at construction time
/// </summary>
public class DefaultElementTagMapper : IElementTagMapper
{
    #region Private Data

    /// <summary>
    /// Built-in XAML-to-HTML tag mappings. Static and immutable.
    /// </summary>
    private static readonly Dictionary<string, string> s_BuiltInMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        // Layout containers
        { "Grid", "div" },
        { "StackPanel", "div" },
        { "WrapPanel", "div" },
        { "DockPanel", "div" },
        { "Canvas", "div" },
        { "Border", "div" },
        { "ContentControl", "div" },

        // Navigation and grouping
        { "Menu", "nav" },
        { "MenuItem", "li" },
        { "TreeView", "ul" },
        { "TreeViewItem", "li" },

        // Buttons and toggles
        { "Button", "button" },
        { "ToggleButton", "button" },
        { "RepeatButton", "button" },

        // Input controls
        { "TextBox", "input" },
        { "PasswordBox", "input" },
        { "CheckBox", "input" },
        { "RadioButton", "input" },
        { "Slider", "input" },
        { "DatePicker", "input" },

        // Select-based controls
        { "ComboBox", "select" },
        { "ListBox", "select" },
        { "ComboBoxItem", "option" },
        { "ListBoxItem", "option" },

        // Text and display
        { "TextBlock", "span" },
        { "Label", "label" },
        { "Hyperlink", "a" },

        // Data and media
        { "DataGrid", "table" },
        { "ProgressBar", "progress" },
        { "RichTextBox", "div" },
        { "MediaElement", "video" },

        // Containers with semantic meaning
        { "GroupBox", "fieldset" },
        { "Expander", "details" },
        { "Separator", "hr" },

        // Framing
        { "Frame", "iframe" },

        // Image
        { "Image", "img" }
    };

    /// <summary>
    /// Custom mappings provided by callers, allowing extension without modification.
    /// These take priority over built-in mappings.
    /// </summary>
    private readonly Dictionary<string, string> v_Overrides;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance with default built-in mappings only.
    /// </summary>
    public DefaultElementTagMapper() : this(Array.Empty<KeyValuePair<string, string>>())
    {
    }

    /// <summary>
    /// Initializes a new instance with default built-in mappings plus custom overrides.
    /// Enables extension without modification (Open/Closed Principle).
    /// </summary>
    /// <param name="overrides">Custom XAML-to-HTML mappings that override built-ins.</param>
    public DefaultElementTagMapper(IEnumerable<KeyValuePair<string, string>> overrides)
    {
        v_Overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in overrides)
        {
            v_Overrides[kvp.Key] = kvp.Value;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the HTML tag name that corresponds to the given XAML element type.
    /// Checks custom overrides first, then built-in mappings, then returns "div" as fallback.
    /// </summary>
    /// <param name="xamlType">The XAML element type name to look up.</param>
    /// <returns>The corresponding HTML tag name, or "div" if no mapping exists.</returns>
    public string Map(string xamlType)
    {
        // Priority 1: Custom overrides
        if (v_Overrides.TryGetValue(xamlType, out var overrideTag))
            return overrideTag;

        // Priority 2: Built-in mappings
        if (s_BuiltInMappings.TryGetValue(xamlType, out var tag))
            return tag;

        // Fallback: Always safe to use div
        return "div";
    }

    #endregion
}