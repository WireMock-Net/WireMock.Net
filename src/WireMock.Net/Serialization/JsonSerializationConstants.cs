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
    }
}