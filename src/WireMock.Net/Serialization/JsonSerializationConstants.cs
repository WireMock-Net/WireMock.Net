using Newtonsoft.Json;

namespace WireMock.Serialization
{
    internal static class JsonSerializationConstants
    {
        public static readonly JsonSerializerSettings JsonSerializerSettingsDefault = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static readonly JsonSerializerSettings JsonSerializerSettingsIncludeNullValues = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include
        };

        public static readonly JsonSerializerSettings JsonDeserializerSettingsWithDateParsingNone = new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.None
        };

        public static readonly JsonSerializerSettings JsonSerializerSettingsPact = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}