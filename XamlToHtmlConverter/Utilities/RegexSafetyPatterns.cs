// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text.RegularExpressions;

namespace XamlToHtmlConverter.Utilities;

/// <summary>
/// Provides safe regex operations with mandatory timeout protection
/// to prevent Regular Expression Denial of Service (ReDoS) attacks.
/// 
/// All regex patterns in this application should use RegexOptions.Compiled
/// and specify a TimeSpan timeout to protect against catastrophic backtracking.
/// 
/// Resources:
/// - OWASP: https://owasp.org/www-community/Regular_expression_Denial_of_Service_-_ReDoS
/// - CWE-1333: Inefficient Regular Expression Complexity
/// </summary>
public static class RegexSafetyPatterns
{
    /// <summary>
    /// The default timeout for regex operations (500 milliseconds).
    /// Adjust based on expected input sizes and complexity.
    /// Keep under 1 second to prevent user-facing delays.
    /// </summary>
    public static readonly TimeSpan DefaultRegexTimeout = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Creates a compiled regex with mandatory timeout protection.
    /// ALWAYS use this helper when creating new Regex patterns.
    /// </summary>
    /// <param name="pattern">The regex pattern string.</param>
    /// <param name="options">Optional regex options (default: None). Compiled flag is always applied.</param>
    /// <param name="timeout">Optional timeout duration (default: 500ms).</param>
    /// <returns>A compiled Regex with timeout protection.</returns>
    /// <example>
    /// // Correct usage (RECOMMENDED):
    /// var emailRegex = RegexSafetyPatterns.CreateSafeRegex(@"^[\w\.-]+@[\w\.-]+\.\w+$");
    /// 
    /// // With custom timeout:
    /// var complexRegex = RegexSafetyPatterns.CreateSafeRegex(pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));
    /// </example>
    public static Regex CreateSafeRegex(string pattern, RegexOptions options = RegexOptions.None, TimeSpan? timeout = null)
    {
        timeout ??= DefaultRegexTimeout;
        return new Regex(pattern, options | RegexOptions.Compiled, timeout.Value);
    }

    /// <summary>
    /// Safely matches input against a pattern with protection against ReDoS.
    /// Wraps Regex.IsMatch with timeout handling.
    /// </summary>
    public static bool SafeIsMatch(string input, string pattern, RegexOptions options = RegexOptions.None, TimeSpan? timeout = null)
    {
        var regex = CreateSafeRegex(pattern, options, timeout);
        try
        {
            return regex.IsMatch(input);
        }
        catch (RegexMatchTimeoutException)
        {
            // Log the timeout event and handle gracefully
            // In production, consider logging: LogWarning("Regex timeout on pattern", new { pattern, inputLength = input.Length });
            return false;
        }
    }

    /// <summary>
    /// Safely extracts matches from input with ReDoS protection.
    /// Returns empty collection on timeout.
    /// </summary>
    public static MatchCollection SafeMatches(string input, string pattern, RegexOptions options = RegexOptions.None, TimeSpan? timeout = null)
    {
        var regex = CreateSafeRegex(pattern, options, timeout);
        try
        {
            return regex.Matches(input);
        }
        catch (RegexMatchTimeoutException)
        {
            // Return empty matches collection by matching against empty string
            return Regex.Matches("", "(?!)"); // Pattern that never matches
        }
    }

    /// <summary>
    /// Safely replaces text matching a pattern with replacement.
    /// Returns original input on timeout.
    /// </summary>
    public static string SafeReplace(string input, string pattern, string replacement, RegexOptions options = RegexOptions.None, TimeSpan? timeout = null)
    {
        var regex = CreateSafeRegex(pattern, options, timeout);
        try
        {
            return regex.Replace(input, replacement);
        }
        catch (RegexMatchTimeoutException)
        {
            // Return original input unchanged to prevent data loss on timeout
            return input;
        }
    }
}

/// <summary>
/// Pre-compiled regex patterns with built-in ReDoS protection.
/// Sourced from common XAML-to-HTML conversion scenarios.
/// ALWAYS use these instead of creating inline patterns.
/// </summary>
public static class CommonPatterns
{
    // CSS Property patterns
    public static readonly Regex CssPropertyName = RegexSafetyPatterns.CreateSafeRegex(@"^[a-zA-Z\-]+$", RegexOptions.Compiled);
    public static readonly Regex CssPropertyValue = RegexSafetyPatterns.CreateSafeRegex(@"^[a-zA-Z0-9\-#().,\s%]+$", RegexOptions.Compiled);

    // XAML element type patterns (safe: no backtracking)
    public static readonly Regex XamlElementType = RegexSafetyPatterns.CreateSafeRegex(@"^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);

    // HTML attribute patterns
    public static readonly Regex HtmlAttributeName = RegexSafetyPatterns.CreateSafeRegex(@"^[a-zA-Z\-:]+$", RegexOptions.Compiled);
    public static readonly Regex HtmlAttributeValue = RegexSafetyPatterns.CreateSafeRegex(@"^[^""]*$", RegexOptions.Compiled);

    // Binding expression patterns (simple, no catastrophic backtracking)
    public static readonly Regex SimpleBindingPath = RegexSafetyPatterns.CreateSafeRegex(@"^[a-zA-Z_][a-zA-Z0-9_\.]*$", RegexOptions.Compiled);
}
