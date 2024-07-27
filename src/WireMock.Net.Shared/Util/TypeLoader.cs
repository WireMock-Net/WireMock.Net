// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Stef.Validation;

namespace WireMock.Util;

public static class TypeLoader
{
    private static readonly ConcurrentDictionary<string, Type?> FoundTypes = new();

    public static bool TryFindType<TInterface>([NotNullWhen(true)] out Type? pluginType) where TInterface : class
    {
        var key = typeof(TInterface).FullName!;

        pluginType = FoundTypes.GetOrAdd(key, _ => TryFindTypeInDlls<TInterface>(null, out var foundType) ? foundType : null);

        return pluginType != null;
    }

    public static bool TryFindType<TInterface>(string implementationTypeFullName, [NotNullWhen(true)] out Type? pluginType) where TInterface : class
    {
        var @interface = typeof(TInterface).FullName;
        var key = $"{@interface}_{implementationTypeFullName}";

        pluginType = FoundTypes.GetOrAdd(key, _ => TryFindTypeInDlls<TInterface>(implementationTypeFullName, out var foundType) ? foundType : null);

        return pluginType != null;
    }

    public static TInterface Load<TInterface>(params object?[] args) where TInterface : class
    {
        if (TryFindType<TInterface>(out var pluginType))
        {
            return (TInterface)Activator.CreateInstance(pluginType, args)!;
        }
        
        throw new DllNotFoundException($"No dll found which implements Interface '{typeof(TInterface).FullName}'.");
    }

    public static TInterface LoadByFullName<TInterface>(string implementationTypeFullName, params object?[] args) where TInterface : class
    {
        Guard.NotNullOrEmpty(implementationTypeFullName);

        if (TryFindType<TInterface>(implementationTypeFullName, out var pluginType))
        {
            return (TInterface)Activator.CreateInstance(pluginType, args)!;
        }

        throw new DllNotFoundException($"No dll found which implements Interface '{typeof(TInterface).FullName}' and has FullName '{implementationTypeFullName}'.");
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