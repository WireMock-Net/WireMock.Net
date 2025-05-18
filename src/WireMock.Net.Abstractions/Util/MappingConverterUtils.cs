// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Util;

/// <summary>
/// Some MappingConverter utility methods.
/// </summary>
public static class MappingConverterUtils
{
    /// <summary>
    /// Convert a list of matchers to C# code arguments.
    /// </summary>
    /// <param name="matchers">A list of matchers.</param>
    /// <returns>The C# code arguments as string.</returns>
    public static string ToCSharpCodeArguments(IReadOnlyList<IMatcher> matchers)
    {
        return string.Join(", ", matchers.Select(m => m.GetCSharpCodeArguments()));
    }

    /// <summary>
    /// Convert a list of patterns to C# code arguments.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    /// <returns>The C# code arguments as string.</returns>
    public static string ToCSharpCodeArguments(AnyOf<string, StringPattern>[] patterns)
    {
        return string.Join(", ", patterns.Select(p => CSharpFormatter.ToCSharpStringLiteral(p.GetPattern())));
    }
}