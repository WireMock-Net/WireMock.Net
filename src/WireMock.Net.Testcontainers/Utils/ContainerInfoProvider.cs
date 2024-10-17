// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Runtime.InteropServices;
using WireMock.Net.Testcontainers.Models;

namespace WireMock.Net.Testcontainers.Utils;

internal static class ContainerInfoProvider
{
    public static readonly Dictionary<OSPlatform, ContainerInfo> Info = new()
    {
        { OSPlatform.Linux, new ContainerInfo("sheyenrath/wiremock.net-alpine", "/app/__admin/mappings") },
        { OSPlatform.Windows, new ContainerInfo("sheyenrath/wiremock.net-windows", @"c:\app\__admin\mappings") }
    };
}