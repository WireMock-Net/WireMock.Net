using System.Linq;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Validation;
using JetBrains.Annotations;
using log4net;

namespace WireMock.Net.StandAlone
{
    /// <summary>
    /// The StandAloneApp
    /// </summary>
    public static class StandAloneApp
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StandAloneApp));

        /// <summary>
        /// Start WireMock.Net standalone Server based on the FluentMockServerSettings.
        /// </summary>
        /// <param name="settings">The FluentMockServerSettings</param>
        [PublicAPI]
        public static FluentMockServer Start([NotNull] IFluentMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            return FluentMockServer.Start(settings);
        }

        /// <summary>
        /// Start WireMock.Net standalone Server based on the commandline arguments.
        /// </summary>
        /// <param name="args">The commandline arguments</param>
        [PublicAPI]
        public static FluentMockServer Start([NotNull] string[] args)
        {
            Check.NotNull(args, nameof(args));

            Log.DebugFormat("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

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

            FluentMockServer server = Start(settings);

            Log.InfoFormat("WireMock.Net server listening at {0}", string.Join(",", server.Urls));

            return server;
        }
    }
}