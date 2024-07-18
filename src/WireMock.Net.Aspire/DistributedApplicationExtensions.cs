// Copyright © WireMock.Net

using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestEase;
using WireMock.Client;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Some WireMock.Net extension methods for working with <see cref="DistributedApplication"/>.
/// Based on https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.Testing/DistributedApplicationHostingTestingExtensions.cs
/// </summary>
public static class DistributedApplicationExtensions
{
    /// <summary>
    /// Create a RestEase Admin client which can be used to call the admin REST endpoint.
    /// </summary>
    /// <param name="app">The <see cref="DistributedApplication"/>.</param>
    /// <param name="resourceName">The resourceName of the resource.</param>
    /// <param name="endpointName">The resourceName of the endpoint on the resource to communicate with.</param>
    /// <returns>A <see cref="IWireMockAdminApi"/></returns>
    public static IWireMockAdminApi CreateWireMockAdminClient(this DistributedApplication app, string resourceName, string? endpointName = default)
    {
        ThrowIfNotStarted(app);

        var (resource, endpointUri) = GetResourceAndEndpointUri(app, resourceName);

        var api = RestClient.For<IWireMockAdminApi>(endpointUri);
        if (resource.Arguments.HasBasicAuthentication)
        {
            api.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{resource.Arguments.AdminUsername}:{resource.Arguments.AdminPassword}")));
        }

        return api;
    }

    private static (WireMockServerResource WireMockServerResource, string EndpointUri) GetResourceAndEndpointUri(IHost app, string resourceName, string? endpointName = default)
    {
        var wireMockServerResource = GetWireMockServerResource(app, resourceName);

        EndpointReference? endpoint;
        if (!string.IsNullOrEmpty(endpointName))
        {
            endpoint = GetEndpointOrDefault(wireMockServerResource, endpointName);
        }
        else
        {
            endpoint = GetEndpointOrDefault(wireMockServerResource, "http") ?? GetEndpointOrDefault(wireMockServerResource, "https");
        }

        if (endpoint is null)
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Endpoint '{0}' for resource '{1}' not found.", endpointName, resourceName), nameof(endpointName));
        }

        return (wireMockServerResource, endpoint.Url);
    }

    private static WireMockServerResource GetWireMockServerResource(IHost app, string resourceName)
    {
        var applicationModel = app.Services.GetRequiredService<DistributedApplicationModel>();

        var resource = applicationModel.Resources
            .OfType<WireMockServerResource>()
            .SingleOrDefault(r => string.Equals(r.Name, resourceName, StringComparison.OrdinalIgnoreCase));

        if (resource is null)
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "WireMockServerResource with name '{0}' not found.", resourceName), nameof(resourceName));
        }

        return resource;
    }

    private static EndpointReference? GetEndpointOrDefault(IResourceWithEndpoints wireMockServerResource, string endpointName)
    {
        var reference = wireMockServerResource.GetEndpoint(endpointName);

        return reference.IsAllocated ? reference : null;
    }

    private static void ThrowIfNotStarted(IHost app)
    {
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        if (!lifetime.ApplicationStarted.IsCancellationRequested)
        {
            throw new InvalidOperationException("The application must be started before resolving endpoints or connection strings");
        }
    }
}