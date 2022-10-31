using System;

namespace WireMock.Extensions;

internal static class EnumExtensions
{
    public static string GetFullyQualifiedEnumValue<T>(this T enumValue)
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enum");
        }

        var type = typeof(T);
        return $"{type.Namespace}.{type.Name}.{enumValue}";
    }
}