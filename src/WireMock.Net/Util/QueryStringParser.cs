// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using WireMock.Types;

namespace WireMock.Util;

/// <summary>
/// Based on https://stackoverflow.com/questions/659887/get-url-parameters-from-a-string-in-net
/// </summary>
internal static class QueryStringParser
{
    private static readonly Dictionary<string, WireMockList<string>> Empty = new();

    public static bool TryParse(string? queryString, bool caseIgnore, [NotNullWhen(true)] out IDictionary<string, string>? nameValueCollection)
    {
        if (queryString is null)
        {
            nameValueCollection = default;
            return false;
        }

        var parts = queryString!
            .Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(parameter => parameter.Split('='))
            .Distinct();

        nameValueCollection = caseIgnore ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>();
        foreach (var part in parts)
        {
            if (part.Length == 2)
            {
                nameValueCollection.Add(part[0], WebUtility.UrlDecode(part[1]));
            }
        }

        return true;
    }

    public static IDictionary<string, WireMockList<string>> Parse(string? queryString, QueryParameterMultipleValueSupport? support = null)
    {
        if (string.IsNullOrEmpty(queryString))
        {
            return Empty;
        }

        var queryParameterMultipleValueSupport = support ?? QueryParameterMultipleValueSupport.All;

        var splitOn = new List<string>();
        if (queryParameterMultipleValueSupport.HasFlag(QueryParameterMultipleValueSupport.Ampersand))
        {
            splitOn.Add("&"); // Support "?key=value&key=anotherValue"
        }
        if (queryParameterMultipleValueSupport.HasFlag(QueryParameterMultipleValueSupport.SemiColon))
        {
            splitOn.Add(";"); // Support "?key=value;key=anotherValue"
        }

        return queryString!.TrimStart('?')
            .Split(splitOn.ToArray(), StringSplitOptions.RemoveEmptyEntries)
            .Select(parameter => new { hasEqualSign = parameter.Contains('='), parts = parameter.Split(['='], 2, StringSplitOptions.RemoveEmptyEntries) })
            .GroupBy(x => x.parts[0], y => JoinParts(y.hasEqualSign, y.parts))
            .ToDictionary
            (
                grouping => grouping.Key,
                grouping => new WireMockList<string>(grouping.SelectMany(x => x).Select(WebUtility.UrlDecode).OfType<string>())
            );

        string[] JoinParts(bool hasEqualSign, string[] parts)
        {
            if (parts.Length > 1)
            {
                return queryParameterMultipleValueSupport.HasFlag(QueryParameterMultipleValueSupport.Comma) ?
                    parts[1].Split([","], StringSplitOptions.RemoveEmptyEntries) : // Support "?key=1,2"
                    [parts[1]];
            }

            return hasEqualSign ? [string.Empty] : []; // Return empty string if no value (#1247)
        }
    }
}