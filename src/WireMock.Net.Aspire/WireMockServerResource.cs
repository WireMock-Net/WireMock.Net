// Copyright © WireMock.Net

using RestEase;
using Stef.Validation;
using WireMock.Client;
using WireMock.Client.Extensions;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting.ApplicationModel;

/// <summary>
/// A resource that represents a WireMock.Net Server.
/// </summary>
public class WireMockServerResource : ContainerResource, IResourceWithServiceDiscovery
{
    internal WireMockServerArguments Arguments { get; }

    internal Lazy<IWireMockAdminApi> AdminApi => new(CreateWireMockAdminApi);

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockServerResource"/> class.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="arguments">The arguments to start the WireMock.Net Server.</param>
    public WireMockServerResource(string name, WireMockServerArguments arguments) : base(name)
    {
        Arguments = Guard.NotNull(arguments);
    }

    /// <summary>
    /// Gets an endpoint reference.
    /// </summary>
    /// <returns>An <see cref="EndpointReference"/> object representing the endpoint reference.</returns>
    public EndpointReference GetEndpoint()
    {
        return new EndpointReference(this, "http");
    }

    private IWireMockAdminApi CreateWireMockAdminApi()
    {
        var adminApi = RestClient.For<IWireMockAdminApi>(GetEndpoint().Url);
        return Arguments.HasBasicAuthentication ?
            adminApi.WithAuthorization(Arguments.AdminUsername!, Arguments.AdminPassword!) :
            adminApi;
    }
}