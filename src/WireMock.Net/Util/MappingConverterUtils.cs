// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Util;

internal static class MappingConverterUtils
{
    internal static string ToCSharpCodeArguments(IReadOnlyList<IMatcher> matchers)
    {
        return string.Join(", ", matchers.Select(m => m.GetCSharpCodeArguments()));
    }

    internal static string ToCSharpCodeArguments(AnyOf<string, StringPattern>[] patterns)
    {
        return string.Join(", ", patterns.Select(p => CSharpFormatter.ToCSharpStringLiteral(p.GetPattern())));
    }
}