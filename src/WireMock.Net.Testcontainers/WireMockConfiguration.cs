// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using JetBrains.Annotations;

namespace WireMock.Net.Testcontainers;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class WireMockConfiguration : ContainerConfiguration
{
#pragma warning disable CS1591
    public string? Username { get; }

    public string? Password { get; }

    public string? StaticMappingsPath { get; private set; }

    public bool WatchStaticMappings { get; private set; }

    public bool WatchStaticMappingsInSubdirectories { get; private set; }

    public bool HasBasicAuthentication => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);

    public List<string> AdditionalUrls { get; private set; } = [];

    public WireMockConfiguration(string? username = null, string? password = null)
    {
        Username = username;
        Password = password;
    }
#pragma warning restore CS1591

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WireMockConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration) : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WireMockConfiguration(IContainerConfiguration resourceConfiguration) : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WireMockConfiguration(WireMockConfiguration resourceConfiguration) : this(new WireMockConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public WireMockConfiguration(WireMockConfiguration oldValue, WireMockConfiguration newValue) : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
        StaticMappingsPath = BuildConfiguration.Combine(oldValue.StaticMappingsPath, newValue.StaticMappingsPath);
        WatchStaticMappings = BuildConfiguration.Combine(oldValue.WatchStaticMappings, newValue.WatchStaticMappings);
        WatchStaticMappingsInSubdirectories = BuildConfiguration.Combine(oldValue.WatchStaticMappingsInSubdirectories, newValue.WatchStaticMappingsInSubdirectories);
        AdditionalUrls = BuildConfiguration.Combine(oldValue.AdditionalUrls.AsEnumerable(), newValue.AdditionalUrls.AsEnumerable()).ToList();
    }

    /// <summary>
    /// Set the StaticMappingsPath.
    /// </summary>
    /// <param name="path">The path which contains the StaticMappings.</param>
    /// <returns><see cref="WireMockConfiguration"/></returns>
    public WireMockConfiguration WithStaticMappingsPath(string path)
    {
        StaticMappingsPath = path;
        return this;
    }

    /// <summary>
    /// Watch the static mappings.
    /// </summary>
    /// <param name="includeSubDirectories">Also look in SubDirectories.</param>
    /// <returns><see cref="WireMockConfiguration"/></returns>
    public WireMockConfiguration WithWatchStaticMappings(bool includeSubDirectories)
    {
        WatchStaticMappings = true;
        WatchStaticMappingsInSubdirectories = includeSubDirectories;
        return this;
    }

    /// <summary>
    /// An additional Url on which WireMock listens.
    /// </summary>
    /// <param name="url">The url to add.</param>
    /// <returns><see cref="WireMockConfiguration"/></returns>
    public WireMockConfiguration WithAdditionalUrl(string url)
    {
        AdditionalUrls.Add(url);
        return this;
    }
}