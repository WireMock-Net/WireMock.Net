﻿using JetBrains.Annotations;
using System.Linq;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Validation;

namespace WireMock.Net.StandAlone
{
    /// <summary>
    /// The StandAloneApp
    /// </summary>
    public static class StandAloneApp
    {
        /// <summary>
        /// Start WireMock.Net standalone Server based on the IWireMockServerSettings.
        /// </summary>
        /// <param name="settings">The IWireMockServerSettings</param>
        [PublicAPI]
        public static WireMockServer Start([NotNull] IWireMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            var server = WireMockServer.Start(settings);

            settings.Logger?.Info("WireMock.Net server listening at {0}", string.Join(",", server.Urls));

            return server;
        }

        /// <summary>
        /// Start WireMock.Net standalone Server based on the commandline arguments.
        /// </summary>
        /// <param name="args">The commandline arguments</param>
        /// <param name="logger">The logger</param>
        [PublicAPI]
        public static WireMockServer Start([NotNull] string[] args, [CanBeNull] IWireMockLogger logger = null)
        {
            Check.NotNull(args, nameof(args));

            var settings = WireMockServerSettingsParser.ParseArguments(args, logger);

            settings.Logger?.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            return Start(settings);
        }
    }
}