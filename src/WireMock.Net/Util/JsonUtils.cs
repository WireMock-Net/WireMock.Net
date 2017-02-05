using Newtonsoft.Json.Linq;

namespace WireMock.Util
{
    internal static class JsonUtils
    {
        public static T ParseJTokenToObject<T>(object value)
        {
            if (value == null)
                return default(T);

            JToken token = value as JToken;
            if (token == null)
                return default(T);

            return token.ToObject<T>();
        }
    }
}