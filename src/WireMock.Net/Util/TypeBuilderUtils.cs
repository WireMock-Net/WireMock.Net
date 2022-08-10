using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace WireMock.Util;

/// <summary>
/// Code based on https://stackoverflow.com/questions/40507909/convert-jobject-to-anonymous-object
/// </summary>
internal static class TypeBuilderUtils
{
    private static readonly ConcurrentDictionary<IDictionary<string, Type>, Type> Types = new();

    private static readonly ModuleBuilder ModuleBuilder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName("WireMock.Net.Reflection"), AssemblyBuilderAccess.Run)
            .DefineDynamicModule("WireMock.Net.Reflection.Module");

    public static Type BuildType(IDictionary<string, Type> properties, string? name = null)
    {
        var keyExists = Types.Keys.FirstOrDefault(k => Compare(k, properties));
        if (keyExists != null)
        {
            return Types[keyExists];
        }

        var typeBuilder = GetTypeBuilder(name ?? Guid.NewGuid().ToString());
        foreach (var property in properties)
        {
            CreateGetSetMethods(typeBuilder, property.Key, property.Value);
        }

        var type = typeBuilder.CreateTypeInfo().AsType();

        Types.TryAdd(properties, type);

        return type;
    }

    /// <summary>
    /// https://stackoverflow.com/questions/3804367/testing-for-equality-between-dictionaries-in-c-sharp
    /// </summary>
    private static bool Compare<TKey, TValue>(IDictionary<TKey, TValue> dict1, IDictionary<TKey, TValue> dict2)
    {
        if (dict1 == dict2)
        {
            return true;
        }

        if (dict1.Count != dict2.Count)
        {
            return false;
        }

        var valueComparer = EqualityComparer<TValue>.Default;

        foreach (var kvp in dict1)
        {
            if (!dict2.TryGetValue(kvp.Key, out var value2))
            {
                return false;
            }

            if (!valueComparer.Equals(kvp.Value, value2))
            {
                return false;
            }
        }

        return true;
    }

    private static TypeBuilder GetTypeBuilder(string name)
    {
        return ModuleBuilder.DefineType(name,
            TypeAttributes.Public |
            TypeAttributes.Class |
            TypeAttributes.AutoClass |
            TypeAttributes.AnsiClass |
            TypeAttributes.BeforeFieldInit |
            TypeAttributes.AutoLayout,
            null);
    }

    private static void CreateGetSetMethods(TypeBuilder typeBuilder, string propertyName, Type propertyType)
    {
        var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

        var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

        var getPropertyMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
        var getIl = getPropertyMethodBuilder.GetILGenerator();

        getIl.Emit(OpCodes.Ldarg_0);
        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
        getIl.Emit(OpCodes.Ret);

        var setPropertyMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
        var setIl = setPropertyMethodBuilder.GetILGenerator();
        var modifyProperty = setIl.DefineLabel();

        var exitSet = setIl.DefineLabel();

        setIl.MarkLabel(modifyProperty);
        setIl.Emit(OpCodes.Ldarg_0);
        setIl.Emit(OpCodes.Ldarg_1);
        setIl.Emit(OpCodes.Stfld, fieldBuilder);

        setIl.Emit(OpCodes.Nop);
        setIl.MarkLabel(exitSet);
        setIl.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getPropertyMethodBuilder);
        propertyBuilder.SetSetMethod(setPropertyMethodBuilder);
    }
}