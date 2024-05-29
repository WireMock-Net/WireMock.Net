using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using RestEase;
using WireMock.Client;
using WireMock.Client.Extensions;

namespace WireMock.Net.Aspire;

internal class WireMockServerMappingBuilderHook : IDistributedApplicationLifecycleHook
{
    public Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken)
    {
        var wireMockInstances = appModel.Resources
            .OfType<WireMockServerResource>()
            .ToArray();

        if (!wireMockInstances.Any())
        {
            return Task.CompletedTask;
        }

        foreach (var wireMockInstance in wireMockInstances)
        {
            var endpoint = wireMockInstance.GetEndpoint("http");
            if (endpoint.IsAllocated)
            {
                var adminApi = RestClient.For<IWireMockAdminApi>(endpoint.Url);

                var mappingBuilder = adminApi.GetMappingBuilder();
                wireMockInstance.Arguments.ApiMappingBuilder?.Invoke(mappingBuilder);
            }
        }

        return Task.CompletedTask;
    }
}