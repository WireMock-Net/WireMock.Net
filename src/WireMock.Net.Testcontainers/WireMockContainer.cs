// Copyright © WireMock.Net

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using RestEase;
using Stef.Validation;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Http;
using WireMock.Util;

namespace WireMock.Net.Testcontainers;

/// <summary>
/// A container for running WireMock in a docker environment.
/// </summary>
public sealed class WireMockContainer : DockerContainer
{
    private const int EnhancedFileSystemWatcherTimeoutMs = 2000;
    internal const int ContainerPort = 80;

    private readonly WireMockConfiguration _configuration;

    private IWireMockAdminApi? _adminApi;
    private EnhancedFileSystemWatcher? _enhancedFileSystemWatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public WireMockContainer(WireMockConfiguration configuration) : base(configuration)
    {
        _configuration = Guard.NotNull(configuration);

        Started += WireMockContainer_Started;
    }

    /// <summary>
    /// Gets the public Url.
    /// </summary>
    [PublicAPI]
    public string GetPublicUrl() => GetPublicUri().ToString();

    /// <summary>
    /// Create a RestEase Admin client which can be used to call the admin REST endpoint.
    /// </summary>
    /// <returns>A <see cref="IWireMockAdminApi"/></returns>
    [PublicAPI]
    public IWireMockAdminApi CreateWireMockAdminClient()
    {
        ValidateIfRunning();

        var api = RestClient.For<IWireMockAdminApi>(GetPublicUri());
        return _configuration.HasBasicAuthentication ? api.WithAuthorization(_configuration.Username!, _configuration.Password!) : api;
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

        var client = HttpClientFactory2.Create(handlers);
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

        var client = HttpClientFactory2.Create(innerHandler, handlers);
        client.BaseAddress = GetPublicUri();
        return client;
    }

    /// <inheritdoc />
    protected override ValueTask DisposeAsyncCore()
    {
        if (_enhancedFileSystemWatcher != null)
        {
            _enhancedFileSystemWatcher.EnableRaisingEvents = false;
            _enhancedFileSystemWatcher.Created -= FileCreatedChangedOrDeleted;
            _enhancedFileSystemWatcher.Changed -= FileCreatedChangedOrDeleted;
            _enhancedFileSystemWatcher.Deleted -= FileCreatedChangedOrDeleted;

            _enhancedFileSystemWatcher.Dispose();
            _enhancedFileSystemWatcher = null;
        }

        Started -= WireMockContainer_Started;

        return base.DisposeAsyncCore();
    }

    private void ValidateIfRunning()
    {
        if (State != TestcontainersStates.Running)
        {
            throw new InvalidOperationException("Unable to create HttpClient because the WireMock.Net is not yet running.");
        }
    }

    private void WireMockContainer_Started(object sender, EventArgs e)
    {
        if (sender is not WireMockContainer container)
        {
            return;
        }

        if (container._configuration.WatchStaticMappings && !string.IsNullOrEmpty(container._configuration.StaticMappingsPath))
        {
            _adminApi = container.CreateWireMockAdminClient();

            _enhancedFileSystemWatcher = new EnhancedFileSystemWatcher(container._configuration.StaticMappingsPath!, "*.json", EnhancedFileSystemWatcherTimeoutMs)
            {
                IncludeSubdirectories = container._configuration.WatchStaticMappingsInSubdirectories
            };
            _enhancedFileSystemWatcher.Created += FileCreatedChangedOrDeleted;
            _enhancedFileSystemWatcher.Changed += FileCreatedChangedOrDeleted;
            _enhancedFileSystemWatcher.Deleted += FileCreatedChangedOrDeleted;
            _enhancedFileSystemWatcher.EnableRaisingEvents = true;
        }
    }

    private async void FileCreatedChangedOrDeleted(object sender, FileSystemEventArgs args)
    {
        if (_adminApi == null)
        {
            return;
        }

        Logger.LogInformation("MappingFile created, changed or deleted: '{0}'. Triggering ReloadStaticMappings.", args.FullPath);
        try
        {
            await _adminApi.ReloadStaticMappingsAsync();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error calling /__admin/mappings/reloadStaticMappings");
        }
    }

    private Uri GetPublicUri() => new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(ContainerPort)).Uri;
}