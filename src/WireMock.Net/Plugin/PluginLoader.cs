using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WireMock.Plugin;

internal static class PluginLoader
{
    private static readonly ConcurrentDictionary<Type, Type> Assemblies = new();

    public static T Load<T>(params object[] args) where T : class
    {
        var foundType = Assemblies.GetOrAdd(typeof(T), (type) =>
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");

            Type? pluginType = null;
            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName
                    {
                        Name = Path.GetFileNameWithoutExtension(file)
                    });

                    pluginType = GetImplementationTypeByInterface<T>(assembly);
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
                return pluginType;
            }

            throw new DllNotFoundException($"No dll found which implements type '{type}'");
        });

        return (T)Activator.CreateInstance(foundType, args);
    }

    private static Type? GetImplementationTypeByInterface<T>(Assembly assembly)
    {
        return assembly.GetTypes().FirstOrDefault(t => typeof(T).IsAssignableFrom(t) && !t.GetTypeInfo().IsInterface);
    }
}