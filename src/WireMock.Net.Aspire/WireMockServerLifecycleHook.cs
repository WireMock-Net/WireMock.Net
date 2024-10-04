// Copyright © WireMock.Net

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using WireMock.Net.Aspire.Extensions;

namespace WireMock.Net.Aspire;

internal class WireMockServerLifecycleHook(ResourceLoggerService loggerService) : IDistributedApplicationLifecycleHook
{
    public async Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        var wireMockServerResources = appModel.Resources
            .OfType<WireMockServerResource>()
            .ToArray();

        foreach (var wireMockServerResource in wireMockServerResources)
        {
            loggerService.SetLogger(wireMockServerResource);

            var endpoint = wireMockServerResource.GetEndpoint();
            if (endpoint.IsAllocated)
            {
                await wireMockServerResource.WaitForHealthAsync(cancellationToken);

                await wireMockServerResource.CallApiMappingBuilderActionAsync(cancellationToken);

                wireMockServerResource.StartWatchingStaticMappings(cancellationToken);
            }
        }
    }
}