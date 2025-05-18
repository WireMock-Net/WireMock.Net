// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Stef.Validation;

namespace WireMock.Util;

internal static class TypeLoader
{
    private static readonly ConcurrentDictionary<string, Type> Assemblies = new();
    private static readonly ConcurrentDictionary<Type, object> Instances = new();

    public static TInterface LoadNewInstance<TInterface>(params object?[] args) where TInterface : class
    {
        var pluginType = GetPluginType<TInterface>();

        return (TInterface)Activator.CreateInstance(pluginType, args)!;
    }

    public static TInterface LoadStaticInstance<TInterface>(params object?[] args) where TInterface : class
    {
        var pluginType = GetPluginType<TInterface>();

        return (TInterface)Instances.GetOrAdd(pluginType, key => Activator.CreateInstance(key, args)!);
    }

    public static TInterface LoadNewInstanceByFullName<TInterface>(string implementationTypeFullName, params object?[] args) where TInterface : class
    {
        Guard.NotNullOrEmpty(implementationTypeFullName);

        var pluginType = GetPluginTypeByFullName<TInterface>(implementationTypeFullName);

        return (TInterface)Activator.CreateInstance(pluginType, args)!;
    }

    public static TInterface LoadStaticInstanceByFullName<TInterface>(string implementationTypeFullName, params object?[] args) where TInterface : class
    {
        Guard.NotNullOrEmpty(implementationTypeFullName);

        var pluginType = GetPluginTypeByFullName<TInterface>(implementationTypeFullName);

        return (TInterface)Instances.GetOrAdd(pluginType, key => Activator.CreateInstance(key, args)!);
    }

    private static Type GetPluginType<TInterface>() where TInterface : class
    {
        var key = typeof(TInterface).FullName!;

        var pluginType = Assemblies.GetOrAdd(key, _ =>
        {
            if (TryFindTypeInDlls<TInterface>(null, out var foundType))
            {
                return foundType;
            }

            throw new DllNotFoundException($"No dll found which implements interface '{key}'.");
        });
        return pluginType;
    }

    private static Type GetPluginTypeByFullName<TInterface>(string implementationTypeFullName) where TInterface : class
    {
        var @interface = typeof(TInterface).FullName;
        var key = $"{@interface}_{implementationTypeFullName}";

        var pluginType = Assemblies.GetOrAdd(key, _ =>
        {
            if (TryFindTypeInDlls<TInterface>(implementationTypeFullName, out var foundType))
            {
                return foundType;
            }

            throw new DllNotFoundException($"No dll found which implements Interface '{@interface}' and has FullName '{implementationTypeFullName}'.");
        });
        return pluginType;
    }

    private static bool TryFindTypeInDlls<TInterface>(string? implementationTypeFullName, [NotNullWhen(true)] out Type? pluginType) where TInterface : class
    {
        foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll"))
        {
            try
            {
                var assembly = Assembly.Load(new AssemblyName
                {
                    Name = Path.GetFileNameWithoutExtension(file)
                });

                if (TryGetImplementationTypeByInterfaceAndOptionalFullName<TInterface>(assembly, implementationTypeFullName, out pluginType))
                {
                    return true;
                }
            }
            catch
            {
                // no-op: just try next .dll
            }
        }

        pluginType = null;
        return false;
    }

    private static bool TryGetImplementationTypeByInterfaceAndOptionalFullName<T>(Assembly assembly, string? implementationTypeFullName, [NotNullWhen(true)] out Type? type)
    {
        type = assembly
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(T).IsAssignableFrom(t) && !t.GetTypeInfo().IsInterface &&
                (implementationTypeFullName == null || string.Equals(t.FullName, implementationTypeFullName, StringComparison.OrdinalIgnoreCase))
            );

        return type != null;
    }
}