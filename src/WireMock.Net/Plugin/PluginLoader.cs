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
            throw new NotSupportedException("Assembly.LoadFile is not supported on .NETStandard 1.3");
#else
            return Assemblies.GetOrAdd(typeof(T), (type) =>
            {
                var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

                Type pluginType = null;
                foreach (var file in files)
                {
                    try
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
                    catch
                    {
                        // no-op: just try next .dll
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