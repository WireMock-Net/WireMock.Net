using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Net.Testcontainers.Models;

namespace WireMock.Net.Testcontainers;

/// <summary>
/// An specific fluent Docker container builder for WireMock.Net
/// </summary>
public sealed class WireMockContainerBuilder : ContainerBuilder<WireMockContainerBuilder, WireMockContainer, WireMockConfiguration>
{
    private readonly Dictionary<bool, ContainerInfo> _info = new()
    {
        { false, new ContainerInfo("sheyenrath/wiremock.net:latest", "/app/__admin/mappings") },
        { true, new ContainerInfo("sheyenrath/wiremock.net-windows:latest", @"c:\app\__admin\mappings") }
    };

    private const string DefaultLogger = "WireMockConsoleLogger";

    private readonly Lazy<Task<bool>> _isWindowsAsLazy = new(async () =>
    {
        using var dockerClientConfig = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration();
        using var dockerClient = dockerClientConfig.CreateClient();

        var version = await dockerClient.System.GetVersionAsync();
        return version.Os.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) > -1;
    });

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder" /> class.
    /// </summary>
    public WireMockContainerBuilder() : this(new WireMockConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Automatically set the correct image (Linux or Windows) for WireMock which to create the container.
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithImage()
    {
        var isWindows = _isWindowsAsLazy.Value.GetAwaiter().GetResult();
        return WithImage(_info[isWindows].Image);
    }

    /// <summary>
    /// Set the admin username. and password for the container.
    /// </summary>
    /// <param name="username">The admin username.</param>
    /// <param name="password">The admin password.</param>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    public WireMockContainerBuilder WithAdminUserNameAndPassword(string username, string password)
    {
        Guard.NotNull(username);
        Guard.NotNull(password);

        if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        {
            return this;
        }

        return Merge(DockerResourceConfiguration, new WireMockConfiguration(username, password))
            .WithCommand($"--AdminUserName {username}", $"--AdminPassword {password}");
    }

    /// <summary>
    /// Use the WireMockNullLogger.
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithNullLogger()
    {
        return WithCommand("--WireMockLogger WireMockNullLogger");
    }

    /// <summary>
    /// Defines if the static mappings should be read at startup (default set to false).
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithReadStaticMappings()
    {
        return WithCommand("--ReadStaticMappings true");
    }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithWatchStaticMappings(bool includeSubDirectories)
    {
        return WithCommand("--WatchStaticMappings true").WithCommand($"--WatchStaticMappingsInSubdirectories {includeSubDirectories}");
    }

    /// <summary>
    /// Specifies the path for the (static) mapping json files.
    /// </summary>
    /// <param name="path">The path</param>
    /// <returns></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithMappings(string path)
    {
        Guard.NotNullOrEmpty(path);

        var isWindows = _isWindowsAsLazy.Value.GetAwaiter().GetResult();

        return WithReadStaticMappings().WithBindMount(path, _info[isWindows].MappingsPath);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockContainerBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private WireMockContainerBuilder(WireMockConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override WireMockConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override WireMockContainer Build()
    {
        Validate();

        return new WireMockContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Init()
    {
        var builder = base.Init();

        // In case no image has been set, set the image using internal logic.
        if (builder.DockerResourceConfiguration.Image == null)
        {
            builder = builder.WithImage();
        }

        var isWindows = _isWindowsAsLazy.Value.GetAwaiter().GetResult();
        var waitForContainerOS = isWindows ? Wait.ForWindowsContainer() : Wait.ForUnixContainer();
        return builder
            .WithPortBinding(WireMockContainer.ContainerPort, true)
            .WithCommand($"--WireMockLogger {DefaultLogger}")
            .WithWaitStrategy(waitForContainerOS.UntilMessageIsLogged("By Stef Heyenrath"));
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Merge(WireMockConfiguration oldValue, WireMockConfiguration newValue)
    {
        return new WireMockContainerBuilder(new WireMockConfiguration(oldValue, newValue));
    }
}