// Copyright © WireMock.Net

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;
using RestEase;
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
                var adminApi = CreateWireMockAdminApi(wireMockServerResource);

                var logger = loggerService.GetLogger(wireMockServerResource);
                logger.LogInformation("Checking Health status from WireMock.Net");

                await adminApi.WaitForHealthAsync(cancellationToken: cancellationToken);

                logger.LogInformation("Calling ApiMappingBuilder to add mappings to WireMock.Net");
                var mappingBuilder = adminApi.GetMappingBuilder();
                await wireMockServerResource.Arguments.ApiMappingBuilder!.Invoke(mappingBuilder);
            }
        }
    }

    private static IWireMockAdminApi CreateWireMockAdminApi(WireMockServerResource resource)
    {
        var adminApi = RestClient.For<IWireMockAdminApi>(resource.GetEndpoint().Url);
        return resource.Arguments.HasBasicAuthentication ?
            adminApi.WithAuthorization(resource.Arguments.AdminUsername!, resource.Arguments.AdminPassword!) :
            adminApi;
    }
}