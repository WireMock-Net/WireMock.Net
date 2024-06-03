using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using RestEase;
using WireMock.Client;
using WireMock.Client.Extensions;

namespace WireMock.Net.Aspire;

internal class WireMockServerMappingBuilderHook(ResourceLoggerService loggerService) : IDistributedApplicationLifecycleHook
{
    private const int MaxRetries = 5;

    public async Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken)
    {
        

        var wireMockInstances = appModel.Resources
            .OfType<WireMockServerResource>()
            .Where(i => i.Arguments.ApiMappingBuilder is not null)
            .ToArray();

        if (!wireMockInstances.Any())
        {
            return;
        }

        foreach (var wireMockInstance in wireMockInstances)
        {
            var endpoint = wireMockInstance.GetEndpoint("http");
            if (endpoint.IsAllocated)
            {
                var logger = loggerService.GetLogger(wireMockInstance);
                logger.LogInformation("Checking Health status from WireMock.Net");

                var adminApi = await WaitForMappingsAsync(endpoint, cancellationToken);

                logger.LogInformation("Calling ApiMappingBuilder to add mappings to WireMock.Net");
                var mappingBuilder = adminApi.GetMappingBuilder();
                await wireMockInstance.Arguments.ApiMappingBuilder!.Invoke(mappingBuilder);
            }
        }
    }

    //private static async Task<IWireMockAdminApi> WaitForHealthAsync(EndpointReference endpoint, CancellationToken cancellationToken)
    //{
    //    var adminApi = RestClient.For<IWireMockAdminApi>(endpoint.Url);

    //    var retries = 0;
    //    var healthStatusResponse = await adminApi.GetHealthAsync(cancellationToken);
    //    while ((!healthStatusResponse.ResponseMessage.IsSuccessStatusCode || healthStatusResponse.GetContent() != nameof(HealthStatus.Healthy)) && retries < MaxRetries)
    //    {
    //        await Task.Delay(1000, cancellationToken);
    //        healthStatusResponse = await adminApi.GetHealthAsync(cancellationToken);
    //        retries++;
    //    }

    //    return adminApi;
    //}

    private static async Task<IWireMockAdminApi> WaitForMappingsAsync(EndpointReference endpoint, CancellationToken cancellationToken)
    {
        var adminApi = RestClient.For<IWireMockAdminApi>(endpoint.Url);

        var retries = 0;
        var mappingsOk = await GetMappingsAsync(adminApi, cancellationToken);
        while (!mappingsOk && retries < MaxRetries)
        {
            await Task.Delay(1000, cancellationToken);
            mappingsOk = await GetMappingsAsync(adminApi, cancellationToken);
            retries++;
        }

        return adminApi;
    }

    private static async Task<bool> GetMappingsAsync(IWireMockAdminApi adminApi, CancellationToken cancellationToken)
    {
        try
        {
            _ = await adminApi.GetMappingsAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}