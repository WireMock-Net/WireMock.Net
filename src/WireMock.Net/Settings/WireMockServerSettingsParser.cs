using JetBrains.Annotations;
using WireMock.Logging;
using WireMock.Validation;

namespace WireMock.Settings
{
    /// <summary>
    /// A static helper class to parse commandline arguments into IWireMockServerSettings.
    /// </summary>
    public static class WireMockServerSettingsParser
    {
        /// <summary>
        /// Parse commandline arguments into IWireMockServerSettings.
        /// </summary>
        /// <param name="args">The commandline arguments</param>
        [PublicAPI]
        public static IWireMockServerSettings ParseArguments([NotNull] params string[] args)
        {
            return ParseArguments(args, null);
        }

        /// <summary>
        /// Parse commandline arguments into IWireMockServerSettings.
        /// </summary>
        /// <param name="args">The commandline arguments</param>
        /// <param name="logger">The logger</param>
        [PublicAPI]
        public static IWireMockServerSettings ParseArguments([NotNull] string[] args, [CanBeNull] IWireMockLogger logger = null)
        {
            Check.HasNoNulls(args, nameof(args));

            var parser = new SimpleCommandLineParser();
            parser.Parse(args);

            var settings = new WireMockServerSettings
            {
                StartAdminInterface = parser.GetBoolValue("StartAdminInterface", true),
                ReadStaticMappings = parser.GetBoolValue("ReadStaticMappings"),
                WatchStaticMappings = parser.GetBoolValue("WatchStaticMappings"),
                AllowPartialMapping = parser.GetBoolValue("AllowPartialMapping", false),
                AdminUsername = parser.GetStringValue("AdminUsername"),
                AdminPassword = parser.GetStringValue("AdminPassword"),
                MaxRequestLogCount = parser.GetIntValue("MaxRequestLogCount"),
                RequestLogExpirationDuration = parser.GetIntValue("RequestLogExpirationDuration")
            };

            if (logger != null)
            {
                settings.Logger = logger;
            }

            if (parser.GetStringValue("WireMockLogger") == "WireMockConsoleLogger")
            {
                settings.Logger = new WireMockConsoleLogger();
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
                    BlackListedHeaders = parser.GetValues("BlackListedHeaders"),
                    BlackListedCookies = parser.GetValues("BlackListedCookies")
                };
            }

            return settings;
        }
    }
}