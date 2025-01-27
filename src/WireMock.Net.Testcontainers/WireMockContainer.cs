// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using RestEase;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Http;
using WireMock.Net.Testcontainers.Utils;
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
        _configuration = Stef.Validation.Guard.NotNull(configuration);

        Started += WireMockContainer_Started;
    }

    /// <summary>
    /// Gets the public Url.
    /// </summary>
    [PublicAPI]
    public string GetPublicUrl() => GetPublicUri().ToString();

    /// <summary>
    /// Gets the public Url.
    /// </summary>
    [PublicAPI]
    public IDictionary<int, string> GetPublicUrls() => GetPublicUris().ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

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

    /// <summary>
    /// Copies a test host directory or file to the container and triggers a reload of the static mappings if required.
    /// </summary>
    /// <param name="source">The source directory or file to be copied.</param>
    /// <param name="target">The target directory path to copy the files to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the directory or file has been copied.</returns>
    public new async Task CopyAsync(string source, string target, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default)
    {
        await base.CopyAsync(source, target, fileMode, ct);

        if (_configuration.WatchStaticMappings && await PathStartsWithContainerMappingsPath(target))
        {
            await ReloadStaticMappingsAsync(target, ct);
        }
    }

    /// <summary>
    /// Reload the static mappings.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    public async Task ReloadStaticMappingsAsync(CancellationToken cancellationToken = default)
    {
        if (_adminApi == null)
        {
            return;
        }

        try
        {
            await _adminApi.ReloadStaticMappingsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error calling /__admin/mappings/reloadStaticMappings");
        }
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

    private static async Task<bool> PathStartsWithContainerMappingsPath(string value)
    {
        var imageOs = await TestcontainersUtils.GetImageOSAsync.Value;

        return value.StartsWith(ContainerInfoProvider.Info[imageOs].MappingsPath);
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
        _adminApi = CreateWireMockAdminClient();

        if (!_configuration.WatchStaticMappings || string.IsNullOrEmpty(_configuration.StaticMappingsPath))
        {
            return;
        }

        _enhancedFileSystemWatcher = new EnhancedFileSystemWatcher(_configuration.StaticMappingsPath!, "*.json", EnhancedFileSystemWatcherTimeoutMs)
        {
            IncludeSubdirectories = _configuration.WatchStaticMappingsInSubdirectories
        };
        _enhancedFileSystemWatcher.Created += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.Changed += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.Deleted += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.EnableRaisingEvents = true;
    }

    private async void FileCreatedChangedOrDeleted(object sender, FileSystemEventArgs args)
    {
        try
        {
            await ReloadStaticMappingsAsync(args.FullPath);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error reloading static mappings from '{FullPath}'.", args.FullPath);
        }
    }

    private async Task ReloadStaticMappingsAsync(string path, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("MappingFile created, changed or deleted: '{Path}'. Triggering ReloadStaticMappings.", path);
        await ReloadStaticMappingsAsync(cancellationToken);
    }

    private Uri GetPublicUri() => GetPublicUris()[ContainerPort];

    private IDictionary<int, Uri> GetPublicUris()
    {
        var dict = new Dictionary<int, Uri>
        {
            { ContainerPort, new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(ContainerPort)).Uri }
        };

        foreach (var port in _configuration.ExposedPorts.Keys.Select(int.Parse).Where(p => p != ContainerPort).OrderBy(p => p))
        {
            dict[port] = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(port)).Uri;
        }

        foreach (var url in _configuration.AdditionalUrls)
        {
            if (PortUtils.TryExtract(url, out _, out _, out _, out _, out var port))
            {
                dict[port] = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(port)).Uri;
            }
        }

        return dict;
    }
}