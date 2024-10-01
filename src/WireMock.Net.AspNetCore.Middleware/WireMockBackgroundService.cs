// Copyright © WireMock.Net

using Microsoft.Extensions.Hosting;
using Stef.Validation;
using WireMock.Server;

namespace WireMock.Net.AspNetCore.Middleware;

/// <summary>
/// A <see cref="BackgroundService"/> used to start/stop the <see cref="WireMockServer"/>
/// </summary>
internal class WireMockBackgroundService : BackgroundService
{
    private readonly WireMockServerInstance _serverInstance;

    /// <summary>
    /// Creates a new <see cref="BackgroundService"/> using an instance
    /// of <see cref="WireMockServerInstance"/>
    /// </summary>
    /// <param name="serverInstance"></param>
    public WireMockBackgroundService(WireMockServerInstance serverInstance)
    {
        _serverInstance = Guard.NotNull(serverInstance);
    }

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serverInstance.Start();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _serverInstance.Stop();
        return base.StopAsync(cancellationToken);
    }
}