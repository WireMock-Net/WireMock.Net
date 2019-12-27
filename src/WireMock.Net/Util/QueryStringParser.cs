using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using WireMock.Types;

namespace WireMock.Util
{
    /// <summary>
    /// Based on https://stackoverflow.com/questions/659887/get-url-parameters-from-a-string-in-net
    /// </summary>
    internal static class QueryStringParser
    {
        public static IDictionary<string, WireMockList<string>> Parse(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return new Dictionary<string, WireMockList<string>>();
            }

            string[] JoinParts(string[] parts)
            {
                if (parts.Length > 1)
                {
                    return parts[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries); // support "?key=1,2"
                }

                return new string[0];
            }

            return queryString.TrimStart('?')
                .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries) // Support "?key=value;key=anotherValue" and "?key=value&key=anotherValue"
                .Select(parameter => parameter.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(parts => parts[0], JoinParts)
                .ToDictionary(grouping => grouping.Key, grouping => new WireMockList<string>(grouping.SelectMany(x => x).Select(WebUtility.UrlDecode)));
        }
    }
}