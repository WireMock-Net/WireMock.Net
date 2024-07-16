// Copyright © WireMock.Net

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Exceptions;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.StandAlone;

/// <summary>
/// The StandAloneApp
/// </summary>
public static class StandAloneApp
{
    private static readonly string Version = typeof(StandAloneApp).GetTypeInfo().Assembly.GetName().Version!.ToString();

    /// <summary>
    /// Start WireMock.Net standalone Server based on the WireMockServerSettings.
    /// </summary>
    /// <param name="settings">The WireMockServerSettings</param>
    [PublicAPI]
    public static WireMockServer Start(WireMockServerSettings settings)
    {
        Guard.NotNull(settings);

        var server = WireMockServer.Start(settings);

        settings.Logger?.Info("Version [{0}]", Version);
        settings.Logger?.Info("Server listening at {0}", string.Join(",", server.Urls));

        return server;
    }

    /// <summary>
    /// Start WireMock.Net standalone Server based on the commandline arguments.
    /// </summary>
    /// <param name="args">The commandline arguments</param>
    /// <param name="logger">The logger</param>
    [PublicAPI]
    public static WireMockServer Start(string[] args, IWireMockLogger? logger = null)
    {
        Guard.NotNull(args);

        if (TryStart(args, out var server, logger))
        {
            return server;
        }

        throw new WireMockException($"Unable start start {nameof(WireMockServer)}.");
    }

    /// <summary>
    /// Try to start WireMock.Net standalone Server based on the commandline arguments.
    /// </summary>
    /// <param name="args">The commandline arguments</param>
    /// <param name="logger">The logger</param>
    /// <param name="server">The WireMockServer</param>
    [PublicAPI]
    public static bool TryStart(string[] args, [NotNullWhen(true)] out WireMockServer? server, IWireMockLogger? logger = null)
    {
        Guard.NotNull(args);

        if (WireMockServerSettingsParser.TryParseArguments(args, Environment.GetEnvironmentVariables(), out var settings, logger))
        {
            settings.Logger?.Info("Version [{0}]", Version);
            settings.Logger?.Debug("Server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            server = Start(settings);
            return true;
        }

        server = null;
        return false;
    }
}