using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Stef.Validation;

namespace WireMock.Util;

/// <summary>
/// https://en.wikipedia.org/wiki/HTTP
/// </summary>
public static class HttpVersionParser
{
    private static readonly Regex HttpVersionRegex = new(@"HTTP/(\d+(\.\d+)?)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>
    /// Try to extract the version (as a string) from the protocol.
    /// </summary>
    /// <param name="protocol">The protocol, something like "HTTP/1.1" or "HTTP/2".</param>
    /// <returns>The version ("1.1" or "2") if found and valid, else empty string.</returns>
    public static string Parse(string protocol)
    {
        var match = HttpVersionRegex.Match(Guard.NotNull(protocol));
        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}