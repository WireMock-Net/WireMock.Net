using System.Reflection;

namespace WireMock.Net.Tests
{
    public static class TestUtils
    {
        public static T GetPrivateFieldValue<T>(this object obj, string fieldName)
        {
            var field = obj.GetType().GetTypeInfo().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            return (T)field.GetValue(obj);
        }
    }
}