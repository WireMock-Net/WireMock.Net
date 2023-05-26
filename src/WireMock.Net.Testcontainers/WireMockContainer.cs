using System;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace WireMock.Net.Testcontainers;

/// <summary>
/// A container for running WireMock in a docker environment.
/// </summary>
public sealed class WireMockContainer : DockerContainer
{
    internal const int ContainerPort = 80;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public WireMockContainer(IContainerConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the public Url.
    /// </summary>
    [PublicAPI]
    public string GetPublicUrl() => new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(ContainerPort)).Uri.ToString();
}
