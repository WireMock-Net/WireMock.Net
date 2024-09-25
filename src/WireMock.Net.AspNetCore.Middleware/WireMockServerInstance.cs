// Copyright © WireMock.Net

using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.AspNetCore.Middleware;

/// <summary>
/// WireMockServer Instance object
/// </summary>
internal class WireMockServerInstance
{
    private readonly Action<WireMockServer> _configureAction;
    private readonly WireMockServerSettings? _settings;

    /// <summary>
    /// Creates a new instance and provides ability to add configuration
    /// for the start method of <see cref="WireMockServer"/>
    /// </summary>
    public WireMockServerInstance(Action<WireMockServer> configure, WireMockServerSettings? settings = null)
    {
        _configureAction = configure;
        _settings = settings;
    }

    /// <summary>
    /// Instance accessor for the <see cref="WireMockServer" />
    /// </summary>
    public WireMockServer? Instance { get; private set; }

    /// <summary>
    /// Configures and starts <see cref="WireMockServer"/> instance for use.
    /// </summary>
    public void Start()
    {
        Instance = _settings != null ? WireMockServer.Start(_settings) : WireMockServer.Start();

        _configureAction.Invoke(Instance);
    }

    /// <summary>
    /// Stops the <see cref="WireMockServer"/>
    /// </summary>
    public void Stop()
    {
        Instance?.Stop();
    }
}