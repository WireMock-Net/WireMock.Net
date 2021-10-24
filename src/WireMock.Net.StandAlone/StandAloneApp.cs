using System.Linq;
using System.Reflection;
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
    public static class StandAloneApp
    {
        private static readonly string Version = typeof(StandAloneApp).GetTypeInfo().Assembly.GetName().Version.ToString();

        /// <summary>
        /// Start WireMock.Net standalone Server based on the IWireMockServerSettings.
        /// </summary>
        /// <param name="settings">The IWireMockServerSettings</param>
        [PublicAPI]
        public static WireMockServer Start([NotNull] IWireMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

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
        public static WireMockServer Start([NotNull] string[] args, [CanBeNull] IWireMockLogger logger = null)
        {
            Check.NotNull(args, nameof(args));

            if (WireMockServerSettingsParser.TryParseArguments(args, out var settings, logger))
            {
                settings.Logger?.Info("Version [{0}]", Version);
                settings.Logger?.Debug("Server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

                return Start(settings);
            }

            return null;
        }

        /// <summary>
        /// Try to start WireMock.Net standalone Server based on the commandline arguments.
        /// </summary>
        /// <param name="args">The commandline arguments</param>
        /// <param name="logger">The logger</param>
        /// <param name="server">The WireMockServer</param>
        [PublicAPI]
        public static bool TryStart([NotNull] string[] args, out WireMockServer server, [CanBeNull] IWireMockLogger logger = null)
        {
            Check.NotNull(args, nameof(args));

            if (WireMockServerSettingsParser.TryParseArguments(args, out var settings, logger))
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
}