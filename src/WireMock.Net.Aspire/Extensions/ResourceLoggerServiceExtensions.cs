// Copyright © WireMock.Net

using Aspire.Hosting.ApplicationModel;

namespace WireMock.Net.Aspire.Extensions;

internal static class ResourceLoggerServiceExtensions
{
    public static void SetLogger(this ResourceLoggerService resourceLoggerService, WireMockServerResource wireMockServerResource)
    {
        var logger = resourceLoggerService.GetLogger(wireMockServerResource);
        wireMockServerResource.SetLogger(logger);
    }
}