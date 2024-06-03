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
    private const int InitialWaitingTimeInMilliSeconds = 500;

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

                var adminApi = await WaitForHealthAsync(endpoint, cancellationToken);

                logger.LogInformation("Calling ApiMappingBuilder to add mappings to WireMock.Net");
                var mappingBuilder = adminApi.GetMappingBuilder();
                await wireMockInstance.Arguments.ApiMappingBuilder!.Invoke(mappingBuilder);
            }
        }
    }

    private static async Task<IWireMockAdminApi> WaitForHealthAsync(EndpointReference endpoint, CancellationToken cancellationToken)
    {
        var adminApi = RestClient.For<IWireMockAdminApi>(endpoint.Url);

        var retries = 0;
        var isHealthy = await GetHealthAsync(adminApi, cancellationToken);
        while (!isHealthy && retries < MaxRetries)
        {
            await Task.Delay((int)(InitialWaitingTimeInMilliSeconds * Math.Pow(2, retries)), cancellationToken);
            isHealthy = await GetHealthAsync(adminApi, cancellationToken);
            retries++;
        }

        if (retries >= MaxRetries)
        {
            throw new InvalidOperationException($"Unable to check the /__admin/health endpoint after {MaxRetries} retries.");
        }

        return adminApi;
    }

    private static async Task<bool> GetHealthAsync(IWireMockAdminApi adminApi, CancellationToken cancellationToken)
    {
        try
        {
            var status = await adminApi.GetHealthAsync(cancellationToken);
            return status == nameof(HealthStatus.Healthy);
        }
        catch
        {
            return false;
        }
    }
}