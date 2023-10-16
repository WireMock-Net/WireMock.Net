using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace WireMock.Util;

internal static class ReflectionUtils
{
    private const string DynamicModuleName = "WireMockDynamicModule";
    private static readonly AssemblyName AssemblyName = new("WireMockDynamicAssembly");
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

    public static Type CreateType(string typeName, Type baseType)
    {
        // Check if the type has already been created
        if (TypeCache.TryGetValue(typeName, out var existingType))
        {
            return existingType;
        }

        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(DynamicModuleName);

        var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public, baseType);

        // Create the type and cache it
        var newType = typeBuilder.CreateTypeInfo().AsType();
        TypeCache[typeName] = newType;

        return newType;
    }
}