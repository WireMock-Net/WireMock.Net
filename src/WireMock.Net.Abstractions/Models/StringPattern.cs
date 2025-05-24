// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// StringPattern which defines the Pattern as a string, and optionally the filepath pattern file.
/// </summary>
public struct StringPattern
{
    /// <summary>
    /// The pattern as string.
    /// </summary>
    public string Pattern { get; set; }

    /// <summary>
    /// The filepath (optionally)
    /// </summary>
    public string? PatternAsFile { get; set; }
}