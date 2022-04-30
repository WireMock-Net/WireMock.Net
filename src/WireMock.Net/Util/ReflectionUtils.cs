using System;
using System.Reflection;
using System.Reflection.Emit;

namespace WireMock.Util;

/// <summary>
/// Code based on https://stackoverflow.com/questions/40507909/convert-jobject-to-anonymous-object
/// </summary>
internal static class ReflectionUtils
{
    private static readonly ModuleBuilder ModuleBuilder =
        AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("WireMock.Net.Reflection"), AssemblyBuilderAccess.Run)
            .DefineDynamicModule("WireMock.Net.Reflection.Module");

    public static TypeBuilder GetTypeBuilder(string name)
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

    public static void CreateProperty(this TypeBuilder typeBuilder, string propertyName, Type propertyType)
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