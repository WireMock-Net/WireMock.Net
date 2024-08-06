// Copyright © WireMock.Net

using System;
using System.Reflection;
using WireMock.Matchers;

namespace WireMock.Extensions;

internal static class EnumExtensions
{
    public static string GetFullyQualifiedEnumValue<T>(this T enumValue)
        where T : struct, IConvertible
    {
        var type = typeof(T);

        if (!type.GetTypeInfo().IsEnum)
        {
            throw new ArgumentException("T must be an enum");
        }

        return $"{type.Namespace}.{type.Name}.{enumValue}";
    }

    //public static string? ToCSharpArgument(this MatchBehaviour matchBehaviour)
    //{
    //    return matchBehaviour == MatchBehaviour.AcceptOnMatch ? null : matchBehaviour.GetFullyQualifiedEnumValue();
    //}

    //public static string ToCSharpArgument(this MatchBehaviour? matchBehaviour)
    //{
    //    return (matchBehaviour ?? MatchBehaviour.AcceptOnMatch).GetFullyQualifiedEnumValue();
    //}
}