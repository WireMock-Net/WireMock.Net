// Copyright © WireMock.Net

using System;
using System.Runtime.InteropServices;
using DotNet.Testcontainers.Configurations;
using System.Threading.Tasks;

namespace WireMock.Net.Testcontainers.Utils;

internal static class ContainerUtils
{
    public static Lazy<Task<OSPlatform>> GetImageOSAsync = new(async () =>
    {
        if (TestcontainersSettings.OS.DockerEndpointAuthConfig == null)
        {
            throw new InvalidOperationException($"The {nameof(TestcontainersSettings.OS.DockerEndpointAuthConfig)} is null. Check if Docker is started.");
        }

        using var dockerClientConfig = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration();
        using var dockerClient = dockerClientConfig.CreateClient();

        var version = await dockerClient.System.GetVersionAsync();
        return version.Os.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) >= 0 ? OSPlatform.Windows : OSPlatform.Linux;
    });
}