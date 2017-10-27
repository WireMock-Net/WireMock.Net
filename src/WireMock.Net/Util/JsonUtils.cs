using Newtonsoft.Json.Linq;

namespace WireMock.Util
{
    internal static class JsonUtils
    {
        public static T ParseJTokenToObject<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }

            var token = value as JToken;
            return token == null ? default(T) : token.ToObject<T>();
        }
    }
}