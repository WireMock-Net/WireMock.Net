using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using WireMock.Models;

namespace WireMock.Extensions;

internal static class AnyOfExtensions
{
    public static string GetPattern(this AnyOf<string, StringPattern> value)
    {
        return value.IsFirst ? value.First : value.Second.Pattern;
    }

    public static AnyOf<string, StringPattern>[] ToAnyOfPatterns(this IEnumerable<string> patterns)
    {
        return patterns.Select(p => p.ToAnyOfPattern()).ToArray();
    }

    public static AnyOf<string, StringPattern> ToAnyOfPattern(this string pattern)
    {
        return new AnyOf<string, StringPattern>(pattern);
    }
}