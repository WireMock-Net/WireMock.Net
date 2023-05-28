using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using DotNet.Testcontainers.Containers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using RestEase;
using Stef.Validation;
using WireMock.Client;

namespace WireMock.Net.Testcontainers;

/// <summary>
/// A container for running WireMock in a docker environment.
/// </summary>
public sealed class WireMockContainer : DockerContainer
{
    internal const int ContainerPort = 80;

    private readonly WireMockConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public WireMockContainer(WireMockConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
        _configuration = Guard.NotNull(configuration);
    }

    /// <summary>
    /// Gets the public Url.
    /// </summary>
    [PublicAPI]
    public string GetPublicUrl() => GetPublicUri().ToString();

    /// <summary>
    /// Create a RestEase Admin client
    /// </summary>
    /// <returns>A c<see cref="IWireMockAdminApi"/></returns>
    [PublicAPI]
    public IWireMockAdminApi CreateWireMockAdminClient()
    {
        ValidateIfRunning();

        var api = RestClient.For<IWireMockAdminApi>(GetPublicUri());

        if (_configuration.HasBasicAuthentication)
        {
            api.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configuration.Username}:{_configuration.Password}")));
        }

        return api;
    }

    /// <summary>
    /// Create a <see cref="HttpClient"/> which can be used to call this instance.
    /// <param name="handlers">
    /// An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
    /// as an System.Net.Http.HttpRequestMessage travels from the System.Net.Http.HttpClient
    /// to the network and an System.Net.Http.HttpResponseMessage travels from the network
    /// back to System.Net.Http.HttpClient. The handlers are invoked in a top-down fashion.
    /// That is, the first entry is invoked first for an outbound request message but
    /// last for an inbound response message.
    /// </param>
    /// </summary>
    [PublicAPI]
    public HttpClient CreateClient(params DelegatingHandler[] handlers)
    {
        ValidateIfRunning();

        var client = HttpClientFactory.Create(handlers);
        client.BaseAddress = GetPublicUri();
        return client;
    }

    /// <summary>
    /// Create a <see cref="HttpClient"/> (one for each URL) which can be used to call this instance.
    /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
    /// <param name="handlers">
    /// An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
    /// as an System.Net.Http.HttpRequestMessage travels from the System.Net.Http.HttpClient
    /// to the network and an System.Net.Http.HttpResponseMessage travels from the network
    /// back to System.Net.Http.HttpClient. The handlers are invoked in a top-down fashion.
    /// That is, the first entry is invoked first for an outbound request message but
    /// last for an inbound response message.
    /// </param>
    /// </summary>
    [PublicAPI]
    public HttpClient CreateClient(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
    {
        ValidateIfRunning();

        var client = HttpClientFactory.Create(innerHandler, handlers);
        client.BaseAddress = GetPublicUri();
        return client;
    }

    private void ValidateIfRunning()
    {
        if (State != TestcontainersStates.Running)
        {
            throw new InvalidOperationException("Unable to create HttpClient because the WireMock.Net is not yet running.");
        }
    }

    private Uri GetPublicUri() => new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(ContainerPort)).Uri;
}
