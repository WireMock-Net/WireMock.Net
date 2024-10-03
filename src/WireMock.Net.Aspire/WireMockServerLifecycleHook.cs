// Copyright © WireMock.Net

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;
using WireMock.Client;
using WireMock.Client.Extensions;

namespace WireMock.Net.Aspire;

internal class WireMockServerLifecycleHook(ResourceLoggerService loggerService) : IDistributedApplicationLifecycleHook
{
    public async Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        var wireMockServerResources = appModel.Resources
            .OfType<WireMockServerResource>()
            .Where(resource => resource.Arguments.ApiMappingBuilder is not null)
            .ToArray();

        if (wireMockServerResources.Length == 0)
        {
            return;
        }

        foreach (var wireMockServerResource in wireMockServerResources)
        {
            var endpoint = wireMockServerResource.GetEndpoint();
            if (endpoint.IsAllocated)
            {
                var adminApi = wireMockServerResource.AdminApi.Value;
                var logger = loggerService.GetLogger(wireMockServerResource);

                logger.LogInformation("Checking Health status from WireMock.Net");
                await adminApi.WaitForHealthAsync(cancellationToken: cancellationToken);

                await CallApiMappingBuilderActionAsync(logger, adminApi, wireMockServerResource);
            }
        }
    }

    private static async Task CallApiMappingBuilderActionAsync(ILogger logger, IWireMockAdminApi adminApi, WireMockServerResource wireMockServerResource)
    {
        logger.LogInformation("Calling ApiMappingBuilder to add mappings to WireMock.Net");
        var mappingBuilder = adminApi.GetMappingBuilder();
        await wireMockServerResource.Arguments.ApiMappingBuilder!.Invoke(mappingBuilder);
    }
}