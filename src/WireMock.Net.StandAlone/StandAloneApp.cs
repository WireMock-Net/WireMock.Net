using System;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Validation;

namespace WireMock.Net.StandAlone
{
    /// <summary>
    /// The StandAloneApp
    /// </summary>
    [Obsolete("This class will be removed in version 1.1.0")]
    public static class StandAloneApp
    {
        /// <summary>
        /// Start WireMock.Net standalone Server based on the FluentMockServerSettings.
        /// </summary>
        /// <param name="settings">The FluentMockServerSettings</param>
        [PublicAPI]
        public static WireMockServer Start([NotNull] WireMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            var server = WireMockServer.Start(settings);

            settings.Logger.Info("WireMock.Net server listening at {0}", string.Join(",", server.Urls));

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

            var settings = WireMockServerSettingsParser.ParseArguments(args);

            settings.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            return Start(settings);
        }
    }
}