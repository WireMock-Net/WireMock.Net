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

            return !(value is JToken token) ? default(T) : token.ToObject<T>();
        }
    }
}