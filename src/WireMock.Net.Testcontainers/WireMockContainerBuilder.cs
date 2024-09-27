// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Net.Testcontainers.Models;

namespace WireMock.Net.Testcontainers;

/// <summary>
/// A specific fluent Docker container builder for WireMock.Net
/// </summary>
public sealed class WireMockContainerBuilder : ContainerBuilder<WireMockContainerBuilder, WireMockContainer, WireMockConfiguration>
{
    private const string DefaultLogger = "WireMockConsoleLogger";
    private readonly Dictionary<OSPlatform, ContainerInfo> _info = new()
    {
        { OSPlatform.Linux, new ContainerInfo("sheyenrath/wiremock.net-alpine", "/app/__admin/mappings") },
        { OSPlatform.Windows, new ContainerInfo("sheyenrath/wiremock.net-windows", @"c:\app\__admin\mappings") }
    };

    private readonly Lazy<Task<OSPlatform>> _getOSAsLazy = new(async () =>
    {
        if (TestcontainersSettings.OS.DockerEndpointAuthConfig == null)
        {
            throw new InvalidOperationException($"The {nameof(TestcontainersSettings.OS.DockerEndpointAuthConfig)} is null. Check if Docker is started.");
        }

        using var dockerClientConfig = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration();
        using var dockerClient = dockerClientConfig.CreateClient();

        var version = await dockerClient.System.GetVersionAsync();
        return version.Os.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) >= 0 ? OSPlatform.Windows : OSPlatform.Linux;
    });

    private OSPlatform? _imageOS;
    private string? _staticMappingsPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder" /> class.
    /// </summary>
    public WireMockContainerBuilder() : this(new WireMockConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Automatically use the correct image for WireMock.
    /// For Linux this is "sheyenrath/wiremock.net-alpine:latest"
    /// For Windows this is "sheyenrath/wiremock.net-windows:latest"
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithImage()
    {
        _imageOS ??= _getOSAsLazy.Value.GetAwaiter().GetResult();
        return WithImage(_imageOS.Value);
    }

    /// <summary>
    /// Automatically use a Linux image for WireMock. This is "sheyenrath/wiremock.net-alpine:latest"
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithLinuxImage()
    {
        return WithImage(OSPlatform.Linux);
    }

    /// <summary>
    /// Automatically use a Windows image for WireMock. This is "sheyenrath/wiremock.net-windows:latest"
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithWindowsImage()
    {
        return WithImage(OSPlatform.Windows);
    }

    /// <summary>
    /// Set the admin username and password for the container (basic authentication).
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
    /// <param name="includeSubDirectories">Also look in SubDirectories.</param>
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
    /// <param name="includeSubDirectories">Also look in SubDirectories.</param>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithMappings(string path, bool includeSubDirectories = false)
    {
        _staticMappingsPath = Guard.NotNullOrEmpty(path);

        return WithReadStaticMappings().WithCommand($"--WatchStaticMappingsInSubdirectories {includeSubDirectories}");
    }

    private WireMockContainerBuilder(WireMockConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override WireMockConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override WireMockContainer Build()
    {
        var builder = this;

        // In case no image has been set, set the image using internal logic.
        if (DockerResourceConfiguration.Image == null)
        {
            builder = WithImage();
        }

        // In case the _imageOS is not set, determine it from the Image FullName.
        if (_imageOS == null)
        {
            if (builder.DockerResourceConfiguration.Image.FullName.IndexOf("wiremock.net", StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidOperationException();
            }

            _imageOS = builder.DockerResourceConfiguration.Image.FullName.IndexOf("windows", StringComparison.OrdinalIgnoreCase) >= 0 ? OSPlatform.Windows : OSPlatform.Linux;
        }

        if (!string.IsNullOrEmpty(_staticMappingsPath))
        {
            builder = builder.WithBindMount(_staticMappingsPath, _info[_imageOS.Value].MappingsPath);
        }

        builder.Validate();

        return new WireMockContainer(builder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Init()
    {
        var builder = base.Init();

        var waitForContainerOS = _imageOS == OSPlatform.Windows ? Wait.ForWindowsContainer() : Wait.ForUnixContainer();
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

    private WireMockContainerBuilder WithImage(OSPlatform os)
    {
        _imageOS = os;
        return WithImage(_info[os].Image);
    }
}