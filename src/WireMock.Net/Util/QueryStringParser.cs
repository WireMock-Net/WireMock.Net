using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using WireMock.Types;

namespace WireMock.Util;

/// <summary>
/// Based on https://stackoverflow.com/questions/659887/get-url-parameters-from-a-string-in-net
/// </summary>
internal static class QueryStringParser
{
    private static readonly Dictionary<string, WireMockList<string>> Empty = new();

    public static IDictionary<string, WireMockList<string>> Parse(string? queryString, QueryParameterMultipleValueSupport? support = null)
    {
        if (string.IsNullOrEmpty(queryString))
        {
            return Empty;
        }

        var queryParameterMultipleValueSupport = support ?? QueryParameterMultipleValueSupport.All;

        string[] JoinParts(string[] parts)
        {
            if (parts.Length > 1)
            {
                return queryParameterMultipleValueSupport.HasFlag(QueryParameterMultipleValueSupport.Comma) ?
                    parts[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries) : // Support "?key=1,2"
                    new[] { parts[1] };
            }

            return new string[0];
        }

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
            .Select(parameter => parameter.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
            .GroupBy(parts => parts[0], JoinParts)
            .ToDictionary(grouping => grouping.Key, grouping => new WireMockList<string>(grouping.SelectMany(x => x).Select(WebUtility.UrlDecode)));
    }
}