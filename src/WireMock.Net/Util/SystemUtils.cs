// Copyright © WireMock.Net

using System.Reflection;

namespace WireMock.Util;

internal static class SystemUtils
{
    public static readonly string Version = typeof(SystemUtils).GetTypeInfo().Assembly.GetName().Version!.ToString();
}