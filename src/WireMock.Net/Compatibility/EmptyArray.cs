// Copyright © WireMock.Net

// ReSharper disable once CheckNamespace
namespace System;

internal static class EmptyArray<T>
{
#if NET451 || NET452
    public static readonly T[] Value = new T[0];
#else
    public static readonly T[] Value = Array.Empty<T>();
#endif
}