using System.Linq;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Validation;
using JetBrains.Annotations;
using WireMock.Logging;

namespace WireMock.Net.StandAlone
{
    /// <summary>
    /// The StandAloneApp
    /// </summary>
    public static class StandAloneApp
    {
        /// <summary>
        /// Start WireMock.Net standalone Server based on the FluentMockServerSettings.
        /// </summary>
        /// <param name="settings">The FluentMockServerSettings</param>
        [PublicAPI]
        public static FluentMockServer Start([NotNull] IFluentMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            var server = FluentMockServer.Start(settings);

            settings.Logger.Info("WireMock.Net server listening at {0}", string.Join(",", server.Urls));

            return server;
        }

        /// <summary>
        /// Start WireMock.Net standalone Server based on the commandline arguments.
        /// </summary>
        /// <param name="args">The commandline arguments</param>
        /// <param name="logger">The logger</param>
        [PublicAPI]
        public static FluentMockServer Start([NotNull] string[] args, [CanBeNull] IWireMockLogger logger = null)
        {
            Check.NotNull(args, nameof(args));

            var parser = new SimpleCommandLineParser();
            parser.Parse(args);

            var settings = new FluentMockServerSettings
            {
                StartAdminInterface = parser.GetBoolValue("StartAdminInterface", true),
                ReadStaticMappings = parser.GetBoolValue("ReadStaticMappings"),
                WatchStaticMappings = parser.GetBoolValue("WatchStaticMappings"),
                AllowPartialMapping = parser.GetBoolValue("AllowPartialMapping", true),
                AdminUsername = parser.GetStringValue("AdminUsername"),
                AdminPassword = parser.GetStringValue("AdminPassword"),
                MaxRequestLogCount = parser.GetIntValue("MaxRequestLogCount"),
                RequestLogExpirationDuration = parser.GetIntValue("RequestLogExpirationDuration"),
            };

            if (logger != null)
            {
                settings.Logger = logger;
            }

            if (parser.Contains("Port"))
            {
                settings.Port = parser.GetIntValue("Port");
            }
            else
            {
                settings.Urls = parser.GetValues("Urls", new[] { "http://*:9091/" });
            }

            string proxyURL = parser.GetStringValue("ProxyURL");
            if (!string.IsNullOrEmpty(proxyURL))
            {
                settings.ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = proxyURL,
                    SaveMapping = parser.GetBoolValue("SaveMapping"),
                    SaveMappingToFile = parser.GetBoolValue("SaveMappingToFile"),
                    ClientX509Certificate2ThumbprintOrSubjectName = parser.GetStringValue("ClientX509Certificate2ThumbprintOrSubjectName"),
                    BlackListedHeaders = parser.GetValues("BlackListedHeaders")
                };
            }

            settings.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            FluentMockServer server = Start(settings);

            return server;
        }
    }
}