// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Settings;

namespace WireMock.Owin;

internal static class WireMockMiddlewareOptionsHelper
{
    public static IWireMockMiddlewareOptions InitFromSettings(WireMockServerSettings settings, IWireMockMiddlewareOptions? options = null)
    {
        Guard.NotNull(settings);

        options ??= new WireMockMiddlewareOptions();

        options.FileSystemHandler = settings.FileSystemHandler;
        options.PreWireMockMiddlewareInit = settings.PreWireMockMiddlewareInit;
        options.PostWireMockMiddlewareInit = settings.PostWireMockMiddlewareInit;
        options.Logger = settings.Logger;
        options.DisableJsonBodyParsing = settings.DisableJsonBodyParsing;
        options.HandleRequestsSynchronously = settings.HandleRequestsSynchronously;
        options.SaveUnmatchedRequests = settings.SaveUnmatchedRequests;
        options.DoNotSaveDynamicResponseInLogEntry = settings.DoNotSaveDynamicResponseInLogEntry;
        options.QueryParameterMultipleValueSupport = settings.QueryParameterMultipleValueSupport;

        if (settings.CustomCertificateDefined)
        {
            options.X509StoreName = settings.CertificateSettings!.X509StoreName;
            options.X509StoreLocation = settings.CertificateSettings.X509StoreLocation;
            options.X509ThumbprintOrSubjectName = settings.CertificateSettings.X509StoreThumbprintOrSubjectName;
            options.X509CertificateFilePath = settings.CertificateSettings.X509CertificateFilePath;
            options.X509CertificatePassword = settings.CertificateSettings.X509CertificatePassword;
        }

        return options;
    }
}