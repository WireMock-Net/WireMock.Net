// Copyright © WireMock.Net

using Microsoft.Extensions.Hosting;
using Stef.Validation;
using WireMock.Server;

namespace WireMock.Net.AspNetCore.Middleware;

/// <summary>
/// <see cref="BackgroundService"/> used to start/stop the <see cref="WireMockServer"/>
/// </summary>
public class WireMockBackgroundService : BackgroundService
{
    private readonly WireMockServerInstance _server;

    /// <summary>
    /// Creates a new <see cref="BackgroundService"/> using an instance
    /// of <see cref="WireMockServerInstance"/>
    /// </summary>
    /// <param name="server"></param>
    public WireMockBackgroundService(WireMockServerInstance server)
    {
        _server = Guard.NotNull(server);
    }

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _server.Start();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _server.Stop();
        return base.StopAsync(cancellationToken);
    }
}