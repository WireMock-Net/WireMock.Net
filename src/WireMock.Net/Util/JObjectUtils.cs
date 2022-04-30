using System;
using Newtonsoft.Json.Linq;

namespace WireMock.Util;

internal static class JObjectUtils
{
    public static Type CreateType(JObject instance, string? fullName = null)
    {
        var typeBuilder = ReflectionUtils.GetTypeBuilder(fullName ?? Guid.NewGuid().ToString());
        foreach (var item in instance.Properties())
        {
            if (item.Type == JTokenType.Object)
            {
                var type = CreateType((JObject)item.Value);
                if (item.Value.Type != JTokenType.None)
                {
                    typeBuilder.CreateProperty(item.Name, type);
                }
            }
            else
            {
                if (item.Value.Type != JTokenType.None)
                {
                    typeBuilder.CreateProperty(item.Name, ConvertType(item.Value));
                }
            }
        }

        return typeBuilder.CreateTypeInfo().AsType() ?? throw new InvalidOperationException();
    }

    private static Type ConvertType(JToken value)
    {
        var type = value.Type;
        return type switch
        {
            JTokenType.Array => value.HasValues ? ConvertType(value.First!).MakeArrayType() : typeof(object).MakeArrayType(),
            JTokenType.Boolean => typeof(bool),
            JTokenType.Bytes => typeof(byte[]),
            JTokenType.Date => typeof(DateTime),
            JTokenType.Guid => typeof(Guid),
            JTokenType.Float => typeof(float),
            JTokenType.Integer => typeof(long),
            JTokenType.Null => typeof(object),
            JTokenType.Object => typeof(object),
            JTokenType.String => typeof(string),
            JTokenType.TimeSpan => typeof(TimeSpan),
            _ => typeof(object)
        };
    }
}