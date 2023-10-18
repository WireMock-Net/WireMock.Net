using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace WireMock.Util;

internal static class ReflectionUtils
{
    private const string DynamicModuleName = "WireMockDynamicModule";
    private static readonly AssemblyName AssemblyName = new("WireMockDynamicAssembly");
    private const TypeAttributes ClassAttributes =
        TypeAttributes.Public |
        TypeAttributes.Class |
        TypeAttributes.AutoClass |
        TypeAttributes.AnsiClass |
        TypeAttributes.BeforeFieldInit |
        TypeAttributes.AutoLayout;
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();
    /*
     , 
     */

    public static Type CreateType(string typeName, Type? parentType = null)
    {
        // Check if the type has already been created
        if (TypeCache.TryGetValue(typeName, out var existingType))
        {
            return existingType;
        }

        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(DynamicModuleName);

        var typeBuilder = moduleBuilder.DefineType(typeName, ClassAttributes, parentType);

        // Create the type and cache it
        var newType = typeBuilder.CreateTypeInfo().AsType();
        TypeCache[typeName] = newType;

        return newType;
    }

    public static Type CreateGenericType(string typeName, Type genericTypeDefinition, params Type[] typeArguments)
    {
        var key = $"{typeName}_{genericTypeDefinition.Name}_{string.Join(", ", typeArguments.Select(t => t.Name))}";
        if (TypeCache.TryGetValue(key, out var existingType))
        {
            return existingType;
        }

        var genericType = genericTypeDefinition.MakeGenericType(typeArguments);

        // Create the type based on the genericType and cache it
        var newType = CreateType(typeName, genericType);
        TypeCache[key] = newType;

        return newType;
    }
}