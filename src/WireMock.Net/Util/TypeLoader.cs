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

    public static TInterface Load<TInterface>(params object[] args) where TInterface : class
    {
        var key = typeof(TInterface).FullName!;

        var pluginType = Assemblies.GetOrAdd(key, _ =>
        {
            if (TryFindTypeInDlls<TInterface>(null, out var foundType))
            {
                return foundType;
            }

            throw new DllNotFoundException($"No dll found which implements Interface '{key}'.");
        });

        return (TInterface)Activator.CreateInstance(pluginType, args)!;
    }

    public static TInterface LoadByFullName<TInterface>(string implementationTypeFullName, params object[] args) where TInterface : class
    {
        Guard.NotNullOrEmpty(implementationTypeFullName);

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

        return (TInterface)Activator.CreateInstance(pluginType, args)!;
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