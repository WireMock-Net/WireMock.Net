using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WireMock.Plugin
{
    internal static class PluginLoader
    {
        private static ConcurrentDictionary<Type, object> Assemblies = new ConcurrentDictionary<Type, object>();

        public static T Load<T>(params object[] args) where T : class
        {
#if NETSTANDARD1_3
            throw new NotSupportedException();
#else
            return Assemblies.GetOrAdd(typeof(T), (type) =>
            {
                var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");

                Type pluginType = null;
                foreach (var file in files)
                {
                    var assembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), file));

                    pluginType = assembly.GetTypes()
                        .Where(t => typeof(T).IsAssignableFrom(t) && !t.GetTypeInfo().IsInterface)
                        .FirstOrDefault();
                    if (pluginType != null)
                    {
                        break;
                    }
                }

                if (pluginType != null)
                {
                    return Activator.CreateInstance(pluginType, args);
                }

                throw new DllNotFoundException($"No dll found which implements type '{type}'");

            }) as T;
#endif
        }
    }
}