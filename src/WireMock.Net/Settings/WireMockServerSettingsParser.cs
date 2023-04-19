using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Settings;

/// <summary>
/// A static helper class to parse commandline arguments into WireMockServerSettings.
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
    public static bool TryParseArguments(string[] args, [NotNullWhen(true)] out WireMockServerSettings? settings, IWireMockLogger? logger = null)
    {
        Guard.HasNoNulls(args);

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
            AdminAzureADAudience = parser.GetStringValue(nameof(WireMockServerSettings.AdminAzureADAudience)),
            AdminAzureADTenant = parser.GetStringValue(nameof(WireMockServerSettings.AdminAzureADTenant)),
            AdminPassword = parser.GetStringValue("AdminPassword"),
            AdminUsername = parser.GetStringValue("AdminUsername"),
            AllowBodyForAllHttpMethods = parser.GetBoolValue("AllowBodyForAllHttpMethods"),
            AllowCSharpCodeMatcher = parser.GetBoolValue("AllowCSharpCodeMatcher"),
            AllowOnlyDefinedHttpStatusCodeInResponse = parser.GetBoolValue("AllowOnlyDefinedHttpStatusCodeInResponse"),
            AllowPartialMapping = parser.GetBoolValue("AllowPartialMapping"),
            DisableJsonBodyParsing = parser.GetBoolValue("DisableJsonBodyParsing"),
            DisableRequestBodyDecompressing = parser.GetBoolValue(nameof(WireMockServerSettings.DisableRequestBodyDecompressing)),
            DisableDeserializeFormUrlEncoded = parser.GetBoolValue(nameof(WireMockServerSettings.DisableDeserializeFormUrlEncoded)),
            HandleRequestsSynchronously = parser.GetBoolValue("HandleRequestsSynchronously"),
            MaxRequestLogCount = parser.GetIntValue("MaxRequestLogCount"),
            ReadStaticMappings = parser.GetBoolValue("ReadStaticMappings"),
            RequestLogExpirationDuration = parser.GetIntValue("RequestLogExpirationDuration"),
            SaveUnmatchedRequests = parser.GetBoolValue(nameof(WireMockServerSettings.SaveUnmatchedRequests)),
            StartAdminInterface = parser.GetBoolValue("StartAdminInterface", true),
            ThrowExceptionWhenMatcherFails = parser.GetBoolValue("ThrowExceptionWhenMatcherFails"),
            UseRegexExtended = parser.GetBoolValue(nameof(WireMockServerSettings.UseRegexExtended), true),
            WatchStaticMappings = parser.GetBoolValue("WatchStaticMappings"),
            WatchStaticMappingsInSubdirectories = parser.GetBoolValue("WatchStaticMappingsInSubdirectories"),
            HostingScheme = parser.GetEnumValue<HostingScheme>(nameof(WireMockServerSettings.HostingScheme)),
            DoNotSaveDynamicResponseInLogEntry = parser.GetBoolValue(nameof(WireMockServerSettings.DoNotSaveDynamicResponseInLogEntry)),
            QueryParameterMultipleValueSupport = parser.GetEnumValue<QueryParameterMultipleValueSupport>(nameof(WireMockServerSettings.QueryParameterMultipleValueSupport)),
            Culture = parser.GetValue(nameof(WireMockServerSettings.Culture), strings => CultureInfoUtils.Parse(strings.FirstOrDefault()), CultureInfo.CurrentCulture)
        };

#if USE_ASPNETCORE
        settings.CorsPolicyOptions = parser.GetEnumValue(nameof(WireMockServerSettings.CorsPolicyOptions), CorsPolicyOptions.None);
        settings.ClientCertificateMode = parser.GetEnumValue(nameof(WireMockServerSettings.ClientCertificateMode), ClientCertificateMode.NoCertificate);
        settings.AcceptAnyClientCertificate = parser.GetBoolValue(nameof(WireMockServerSettings.AcceptAnyClientCertificate));
#endif

        var loggerType = parser.GetStringValue("WireMockLogger");
        switch (loggerType)
        {
            case nameof(WireMockConsoleLogger):
                settings.Logger = new WireMockConsoleLogger();
                break;

            case nameof(WireMockNullLogger):
                settings.Logger = new WireMockNullLogger();
                break;

            default:
                if (logger != null)
                {
                    settings.Logger = logger;
                }
                break;
        }

        if (parser.Contains(nameof(WireMockServerSettings.Port)))
        {
            settings.Port = parser.GetIntValue(nameof(WireMockServerSettings.Port));
        }
        else if (settings.HostingScheme is null)
        {
            settings.Urls = parser.GetValues("Urls", new[] { "http://*:9091/" });
        }

        var proxyUrl = parser.GetStringValue("ProxyURL") ?? parser.GetStringValue("ProxyUrl");
        if (!string.IsNullOrEmpty(proxyUrl))
        {
            settings.ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                AllowAutoRedirect = parser.GetBoolValue("AllowAutoRedirect"),
                ClientX509Certificate2ThumbprintOrSubjectName = parser.GetStringValue("ClientX509Certificate2ThumbprintOrSubjectName"),
                ExcludedCookies = parser.GetValues("ExcludedCookies"),
                ExcludedHeaders = parser.GetValues("ExcludedHeaders"),
                // PreferProxyMapping = parser.GetBoolValue(nameof(ProxyAndRecordSettings.PreferProxyMapping)),
                SaveMapping = parser.GetBoolValue("SaveMapping"),
                SaveMappingForStatusCodePattern = parser.GetStringValue("SaveMappingForStatusCodePattern", "*"),
                SaveMappingToFile = parser.GetBoolValue("SaveMappingToFile"),
                UseDefinedRequestMatchers = parser.GetBoolValue(nameof(ProxyAndRecordSettings.UseDefinedRequestMatchers)),
                AppendGuidToSavedMappingFile = parser.GetBoolValue(nameof(ProxyAndRecordSettings.AppendGuidToSavedMappingFile)),
                Url = proxyUrl!,
                SaveMappingSettings = new ProxySaveMappingSettings
                {
                    StatusCodePattern = parser.GetStringValue("SaveMappingForStatusCodePattern", "*"),
                    // HttpMethods = new ProxySaveMappingSetting<string[]>(parser.GetValues("DoNotSaveMappingForHttpMethods", new string[0]), MatchBehaviour.RejectOnMatch)
                },
                ReplaceSettings = new ProxyUrlReplaceSettings
                {
                    OldValue = parser.GetStringValue("ProxyUrlReplaceOldValue", ""),
                    NewValue = parser.GetStringValue("ProxyUrlReplaceNewValue", "")
                }
            };

            string? proxyAddress = parser.GetStringValue("WebProxyAddress");
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                settings.ProxyAndRecordSettings.WebProxySettings = new WebProxySettings
                {
                    Address = proxyAddress!,
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