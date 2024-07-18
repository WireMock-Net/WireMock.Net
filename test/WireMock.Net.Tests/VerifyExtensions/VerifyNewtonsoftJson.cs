// Copyright © WireMock.Net

#if !(NET452 || NET461)
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using VerifyTests;

namespace WireMock.Net.Tests.VerifyExtensions;

internal static class VerifyNewtonsoftJson
{
    public static void Enable(VerifySettings verifySettings)
    {
        // InnerVerifier.ThrowIfVerifyHasBeenRun();

        verifySettings
            .AddExtraSettings(_ =>
            {
                var converters = _.Converters;
                converters.Add(new JArrayConverter());
                converters.Add(new JObjectConverter());
            });
    }
}

internal class JArrayConverter : WriteOnlyJsonConverter<JArray>
{
    public override void Write(VerifyJsonWriter writer, JArray value)
    {
        var list = value.ToObject<List<object>>()!;
        writer.Serialize(list);
    }
}

internal class JObjectConverter : WriteOnlyJsonConverter<JObject>
{
    public override void Write(VerifyJsonWriter writer, JObject value)
    {
        var dictionary = value.ToObject<OrderedDictionary>()!;
        writer.Serialize(dictionary);
    }
}
#endif