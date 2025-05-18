// Copyright © WireMock.Net

using System;
using System.Reflection;

namespace WireMock.Extensions;

/// <summary>
/// Some extension methods for Enums.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Get the fully qualified enum value.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="enumValue">The value.</param>
    /// <returns>The fully qualified enum value.</returns>
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
}