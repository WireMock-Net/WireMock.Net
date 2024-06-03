using System.Net.Http.Headers;
using System.Text;
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
        var wireMockServerResources = appModel.Resources
            .OfType<WireMockServerResource>()
            .Where(i => i.Arguments.ApiMappingBuilder is not null)
            .ToArray();

        if (!wireMockServerResources.Any())
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

                await WaitForHealthAsync(adminApi, cancellationToken);

                logger.LogInformation("Calling ApiMappingBuilder to add mappings to WireMock.Net");
                var mappingBuilder = adminApi.GetMappingBuilder();
                await wireMockServerResource.Arguments.ApiMappingBuilder!.Invoke(mappingBuilder);
            }
        }
    }

    private static IWireMockAdminApi CreateWireMockAdminApi(WireMockServerResource resource)
    {
        var adminApi = RestClient.For<IWireMockAdminApi>(resource.GetEndpoint().Url);
        if (resource.Arguments.HasBasicAuthentication)
        {
            adminApi.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{resource.Arguments.AdminUsername}:{resource.Arguments.AdminPassword}")));
        }

        return adminApi;
    }

    private static async Task WaitForHealthAsync(IWireMockAdminApi adminApi, CancellationToken cancellationToken)
    {
        var retries = 0;
        var isHealthy = await GetHealthAsync(adminApi, cancellationToken);
        while (!isHealthy && retries < MaxRetries && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay((int)(InitialWaitingTimeInMilliSeconds * Math.Pow(2, retries)), cancellationToken);
            isHealthy = await GetHealthAsync(adminApi, cancellationToken);
            retries++;
        }

        if (retries >= MaxRetries)
        {
            throw new InvalidOperationException($"Unable to check the /__admin/health endpoint after {MaxRetries} retries.");
        }
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