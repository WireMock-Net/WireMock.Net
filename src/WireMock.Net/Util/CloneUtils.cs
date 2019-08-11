using FastDeepCloner;
using System;
using System.Runtime.Serialization;

namespace WireMock.Util
{
    internal static class CloneUtils
    {
        private static FastDeepClonerSettings settings = new FastDeepCloner.FastDeepClonerSettings()
        {
            FieldType = FastDeepCloner.FieldType.FieldInfo,
            OnCreateInstance = new FastDeepCloner.Extensions.CreateInstance((Type type) =>
            {
#if !NETSTANDARD1_3
                return FormatterServices.GetUninitializedObject(type);
#else
                
#endif
            })
        };

        public static T DeepClone<T>(T objectToBeCloned) where T : class
        {
            //return CloneExtensionsEx.CloneFactory.GetClone(objectToBeCloned);
            // Expression.Lambda<Func<object>>(Expression.New(type)).Compile()
            return FastDeepCloner.DeepCloner.Clone(objectToBeCloned, settings);
        }
    }
}