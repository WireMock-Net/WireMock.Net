using System;
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

        /// <summary>
        /// Set a _private_ Field Value on a given Object
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propertyName">Property name as string.</param>
        /// <param name="value">the value to set</param>
        public static void SetPrivateFieldValue<T>(this object obj, string propertyName, T value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }

            if (fi == null)
            {
                throw new ArgumentOutOfRangeException(nameof(propertyName), $"Field {propertyName} was not found in Type {obj.GetType().FullName}");
            }

            fi.SetValue(obj, value);
        }

        /// <summary>
        /// Sets a _private_ Property Value from a given Object.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is set</param>
        /// <param name="propertyName">Property name as string.</param>
        /// <param name="value">Value to set.</param>
        public static void SetPrivatePropertyValue<T>(this object obj, string propertyName, T value)
        {
            Type t = obj.GetType();
            if (t.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            {
                throw new ArgumentOutOfRangeException(nameof(propertyName), $"Property {propertyName} was not found in Type {obj.GetType().FullName}");
            }

            t.InvokeMember(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { value });
        }
    }
}