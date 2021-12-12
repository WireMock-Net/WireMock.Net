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
        /// Parse commandline arguments into WireMockServerSettings.
        /// </summary>
        /// <param name="args">The commandline arguments</param>
        /// <param name="logger">The logger (optional, can be null)</param>
        /// <param name="settings">The parsed settings</param>
        [PublicAPI]
        public static bool TryParseArguments([NotNull] string[] args, out IWireMockServerSettings settings, [CanBeNull] IWireMockLogger logger = null)
        {
            Check.HasNoNulls(args, nameof(args));

            var parser = new SimpleCommandLineParser();
            parser.Parse(args);

            if (parser.GetBoolSwitchValue("help"))
            {
                (logger ?? new WireMockConsoleLogger()).Info("See https://github.com/WireMock-Net/WireMock.Net/wiki/WireMock-commandline-parameters for details on all commandline options.");
                settings = null;
                return false;
            }

            settings = new WireMockServerSettings
            {
                StartAdminInterface = parser.GetBoolValue("StartAdminInterface", true),
                ReadStaticMappings = parser.GetBoolValue("ReadStaticMappings"),
                WatchStaticMappings = parser.GetBoolValue("WatchStaticMappings"),
                AllowPartialMapping = parser.GetBoolValue("AllowPartialMapping"),
                WatchStaticMappingsInSubdirectories = parser.GetBoolValue("WatchStaticMappingsInSubdirectories"),
                AdminUsername = parser.GetStringValue("AdminUsername"),
                AdminPassword = parser.GetStringValue("AdminPassword"),
                AdminAzureADTenant = parser.GetStringValue(nameof(IWireMockServerSettings.AdminAzureADTenant)),
                AdminAzureADAudience = parser.GetStringValue(nameof(IWireMockServerSettings.AdminAzureADAudience)),
                MaxRequestLogCount = parser.GetIntValue("MaxRequestLogCount"),
                RequestLogExpirationDuration = parser.GetIntValue("RequestLogExpirationDuration"),
                AllowCSharpCodeMatcher = parser.GetBoolValue("AllowCSharpCodeMatcher"),
                AllowBodyForAllHttpMethods = parser.GetBoolValue("AllowBodyForAllHttpMethods"),
                AllowOnlyDefinedHttpStatusCodeInResponse = parser.GetBoolValue("AllowOnlyDefinedHttpStatusCodeInResponse"),
                DisableJsonBodyParsing = parser.GetBoolValue("DisableJsonBodyParsing"),
                HandleRequestsSynchronously = parser.GetBoolValue("HandleRequestsSynchronously"),
                ThrowExceptionWhenMatcherFails = parser.GetBoolValue("ThrowExceptionWhenMatcherFails"),
                UseRegexExtended = parser.GetBoolValue(nameof(IWireMockServerSettings.UseRegexExtended), true)
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

            string proxyUrl = parser.GetStringValue("ProxyURL") ?? parser.GetStringValue("ProxyUrl");
            if (!string.IsNullOrEmpty(proxyUrl))
            {
                settings.ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = proxyUrl,
                    SaveMapping = parser.GetBoolValue("SaveMapping"),
                    SaveMappingToFile = parser.GetBoolValue("SaveMappingToFile"),
                    SaveMappingForStatusCodePattern = parser.GetStringValue("SaveMappingForStatusCodePattern"),
                    ClientX509Certificate2ThumbprintOrSubjectName = parser.GetStringValue("ClientX509Certificate2ThumbprintOrSubjectName"),
                    ExcludedHeaders = parser.GetValues("ExcludedHeaders"),
                    ExcludedCookies = parser.GetValues("ExcludedCookies"),
                    AllowAutoRedirect = parser.GetBoolValue("AllowAutoRedirect")
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

            var certificateSettings = new WireMockCertificateSettings
            {
                X509StoreName = parser.GetStringValue("X509StoreName"),
                X509StoreLocation = parser.GetStringValue("X509StoreLocation"),
                X509StoreThumbprintOrSubjectName = parser.GetStringValue("X509StoreThumbprintOrSubjectName"),
                X509CertificateFilePath = parser.GetStringValue("X509CertificateFilePath"),
                X509CertificatePassword = parser.GetStringValue("X509CertificatePassword")
            };
            if (certificateSettings.IsDefined)
            {
                settings.CertificateSettings = certificateSettings;
            }

            return true;
        }
    }
}