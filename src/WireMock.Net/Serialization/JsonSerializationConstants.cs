// Copyright © WireMock.Net

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WireMock.Serialization;

internal static class JsonSerializationConstants
{
    public static readonly JsonSerializerSettings JsonSerializerSettingsDefault = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore
    };

    public static readonly JsonSerializerSettings JsonSerializerSettingsIncludeNullValues = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Include
    };

    public static readonly JsonSerializerSettings JsonDeserializerSettingsWithDateParsingNone = new()
    {
        DateParseHandling = DateParseHandling.None
    };

    public static readonly JsonSerializerSettings JsonSerializerSettingsPact = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        }
    };
}