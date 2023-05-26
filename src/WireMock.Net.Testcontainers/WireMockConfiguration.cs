using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using JetBrains.Annotations;

namespace WireMock.Net.Testcontainers;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class WireMockConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Gets the admin username.
    /// </summary>
    public string? Username { get; }

    /// <summary>
    /// Gets the admin password.
    /// </summary>
    public string? Password { get; }

    /// <summary>
    /// Gets the logger (WireMockNullLogger or WireMockConsoleLogger).
    /// </summary>
    public string? Logger { get; }

    /// <summary>
    /// Gets the ReadStaticMappings.
    /// </summary>
    public bool ReadStaticMappings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockConfiguration" /> class.
    /// </summary>
    /// <param name="username">The admin username.</param>
    /// <param name="password">The admin password.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="readStaticMappings">The readStaticMappings</param>
    public WireMockConfiguration(string? username = null, string? password = null, string? logger = null, bool readStaticMappings = false)
    {
        Username = username;
        Password = password;
        Logger = logger;
        ReadStaticMappings = readStaticMappings;
    }

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
    }
}