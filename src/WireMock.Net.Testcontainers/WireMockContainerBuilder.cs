using System;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using JetBrains.Annotations;
using Stef.Validation;

namespace WireMock.Net.Testcontainers;

/// <summary>
/// An specific fluent Docker container builder for WireMock.Net
/// </summary>
public sealed class WireMockContainerBuilder : ContainerBuilder<WireMockContainerBuilder, WireMockContainer, WireMockConfiguration>
{
    private const string LinuxImage = "sheyenrath/wiremock.net:latest";
    private const string WindowsImage = "sheyenrath/wiremock.net-windows:latest";
    private const string DefaultLogger = "WireMockConsoleLogger";

    private readonly Lazy<Task<bool>> _isWindowsLazy = new(IsWindowsAsync);

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder" /> class.
    /// </summary>
    public WireMockContainerBuilder() : this(new WireMockConfiguration(logger: DefaultLogger))
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
        var isWindows = _isWindowsLazy.Value.GetAwaiter().GetResult();
        return WithImage(isWindows ? WindowsImage : LinuxImage);
    }

    /// <summary>
    /// Set the admin username and password for the container.
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
    /// Use the WireMockConsoleLogger (default)
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithConsoleLogger()
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(logger: "WireMockConsoleLogger"));
    }

    /// <summary>
    /// Use the WireMockNullLogger.
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithNullLogger()
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(logger: "WireMockNullLogger"));
    }

    /// <summary>
    /// Defines if the static mappings should be read at startup (default set to false).
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithReadStaticMappings()
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(readStaticMappings: true))
            .WithCommand("--ReadStaticMappings true");
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
        return base.Init()
            .WithImage()
            .WithPortBinding(WireMockContainer.ContainerPort, true)
            .WithCommand($"--WireMockLogger {DockerResourceConfiguration.Logger}");
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

    private static async Task<bool> IsWindowsAsync()
    {
        using var dockerClientConfig = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration();
        using var dockerClient = dockerClientConfig.CreateClient();

        var version = await dockerClient.System.GetVersionAsync();
        return version.Os.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) > -1;
    }
}