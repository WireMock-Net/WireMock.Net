﻿using System;
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
        [Obsolete("Will be replaced by WireMockServer.Start(settings) in version 1.1.0")]
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
        [Obsolete("Will be replaced by `var settings = WireMockServerSettingsParser.ParseArguments(args, logger); WireMockServer.Start(settings);` in version 1.1.0")]
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
                AllowPartialMapping = parser.GetBoolValue("AllowPartialMapping"),
                AdminUsername = parser.GetStringValue("AdminUsername"),
                AdminPassword = parser.GetStringValue("AdminPassword"),
                MaxRequestLogCount = parser.GetIntValue("MaxRequestLogCount"),
                RequestLogExpirationDuration = parser.GetIntValue("RequestLogExpirationDuration"),
                AllowCSharpCodeMatcher = parser.GetBoolValue("AllowCSharpCodeMatcher"),
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

            settings.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            return Start(settings);
        }
    }
}