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
                WatchStaticMappingsInSubdirectories = parser.GetBoolValue("WatchStaticMappingsInSubdirectories"),
                AllowPartialMapping = parser.GetBoolValue("AllowPartialMapping"),
                AdminUsername = parser.GetStringValue("AdminUsername"),
                AdminPassword = parser.GetStringValue("AdminPassword"),
                MaxRequestLogCount = parser.GetIntValue("MaxRequestLogCount"),
                RequestLogExpirationDuration = parser.GetIntValue("RequestLogExpirationDuration"),
                AllowCSharpCodeMatcher = parser.GetBoolValue("AllowCSharpCodeMatcher"),
                AllowBodyForAllHttpMethods = parser.GetBoolValue("AllowBodyForAllHttpMethods")
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
                    SaveMappingForStatusCodePattern = parser.GetStringValue("SaveMappingForStatusCodePattern"),
                    ClientX509Certificate2ThumbprintOrSubjectName = parser.GetStringValue("ClientX509Certificate2ThumbprintOrSubjectName"),
                    BlackListedHeaders = parser.GetValues("BlackListedHeaders"),
                    BlackListedCookies = parser.GetValues("BlackListedCookies")
                };

                string proxyAddress = parser.GetStringValue("WebProxyAddress");
                if (!string.IsNullOrEmpty(proxyAddress))
                {
                    settings.ProxyAndRecordSettings.WebProxySettings = new WebProxySettings
                    {
                        Address = proxyAddress,
                        UserName = parser.GetStringValue("WebProxyUserName"),
                        Password = parser.GetStringValue("WebProxyPassword")
                    };
                }
            }

            settings.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            return Start(settings);
        }
    }
}