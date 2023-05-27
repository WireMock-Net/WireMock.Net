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

    public string? Logger { get; }

    public bool ReadStaticMappings { get; }

    public bool WatchStaticMappings { get; }

    public bool WatchStaticMappingsInSubdirectories { get; }
    
    public WireMockConfiguration(
        string? username = null,
        string? password = null,
        string? logger = null,
        bool readStaticMappings = false,
        bool watchStaticMappings = false,
        bool watchStaticMappingsInSubdirectories = false
    )
    {
        Username = username;
        Password = password;
        Logger = logger;
        ReadStaticMappings = readStaticMappings;
        WatchStaticMappings = watchStaticMappings;
        WatchStaticMappingsInSubdirectories = watchStaticMappingsInSubdirectories;
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
        Logger = BuildConfiguration.Combine(oldValue.Logger, newValue.Logger);
        ReadStaticMappings = BuildConfiguration.Combine(oldValue.ReadStaticMappings, newValue.ReadStaticMappings);
        WatchStaticMappings = BuildConfiguration.Combine(oldValue.WatchStaticMappings, newValue.WatchStaticMappings);
        WatchStaticMappingsInSubdirectories = BuildConfiguration.Combine(oldValue.WatchStaticMappingsInSubdirectories, newValue.WatchStaticMappingsInSubdirectories);
    }
}