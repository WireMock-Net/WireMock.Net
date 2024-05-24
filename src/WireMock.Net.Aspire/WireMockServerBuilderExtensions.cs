using System;
using Aspire.Hosting.ApplicationModel;
using Stef.Validation;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding WireMock.Net Server resources to the application model.
/// </summary>
public static class WireMockServerBuilderExtensions
{
    private const int ContainerPort = 80;

    // Linux only (https://github.com/dotnet/aspire/issues/854)
    private const string DefaultLinuxImage = "sheyenrath/wiremock.net";
    private const string DefaultLinuxMappingsPath = "/app/__admin/mappings";

    /// <summary>
    /// Adds a WireMock.Net Server resource to the application model.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="port">The port for the WireMock Server.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> AddWireMock(this IDistributedApplicationBuilder builder, string name, int port = WireMockServerArguments.DefaultPort)
    {
        Guard.NotNull(builder);
        Guard.NotNullOrWhiteSpace(name);
        Guard.Condition(port, p => p > 0);

        return builder.AddWireMock(name, callback =>
        {
            callback.Port = port;
        });
    }

    /// <summary>
    /// Adds a WireMock.Net Server resource to the application model.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="arguments">The arguments to start the WireMock.Net Server.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> AddWireMock(this IDistributedApplicationBuilder builder, string name, WireMockServerArguments arguments)
    {
        Guard.NotNull(builder);
        Guard.NotNullOrWhiteSpace(name);
        Guard.NotNull(arguments);

        var wireMockContainerResource = new WireMockServerResource(name, arguments);
        var resourceBuilder = builder
            .AddResource(wireMockContainerResource)
            .WithImage(DefaultLinuxImage)
            .WithHttpEndpoint(targetPort: ContainerPort, port: arguments.Port);

        if (!string.IsNullOrEmpty(arguments.MappingsPath))
        {
            resourceBuilder = resourceBuilder.WithBindMount(arguments.MappingsPath, DefaultLinuxMappingsPath);
        }

        resourceBuilder = resourceBuilder.WithArgs(context =>
        {
            foreach (var arg in arguments.GetArgs())
            {
                context.Args.Add(arg);
            }
        });

        return resourceBuilder;
    }

    /// <summary>
    /// Adds a WireMock.Net Server resource to the application model.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="callback">A callback that allows for setting the <see cref="WireMockServerArguments"/>.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> AddWireMock(this IDistributedApplicationBuilder builder, string name, Action<WireMockServerArguments> callback)
    {
        Guard.NotNull(builder);
        Guard.NotNullOrWhiteSpace(name);
        Guard.NotNull(callback);

        var arguments = new WireMockServerArguments();
        callback(arguments);

        return builder.AddWireMock(name, arguments);
    }

    /// <summary>
    /// Defines if the static mappings should be read at startup.
    ///
    /// Default set to <c>false</c>.
    /// </summary>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> WithReadStaticMappings(this IResourceBuilder<WireMockServerResource> wiremock)
    {
        Guard.NotNull(wiremock).Resource.Arguments.ReadStaticMappings = true;
        return wiremock;
    }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    ///
    /// Default set to <c>false</c>.
    /// </summary>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> WithWatchStaticMappings(this IResourceBuilder<WireMockServerResource> wiremock)
    {
        Guard.NotNull(wiremock).Resource.Arguments.WithWatchStaticMappings = true;
        return wiremock;
    }

    /// <summary>
    /// Specifies the path for the (static) mapping json files.
    /// </summary>
    /// <param name="wiremock">The <see cref="IResourceBuilder{WireMockServerResource}"/>.</param>
    /// <param name="mappingsPath">The local path.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> WithMappingsPath(this IResourceBuilder<WireMockServerResource> wiremock, string mappingsPath)
    {
        return Guard.NotNull(wiremock)
            .WithBindMount(Guard.NotNullOrWhiteSpace(mappingsPath), DefaultLinuxMappingsPath);
    }

    /// <summary>
    /// Set the admin username and password for WireMock.Net (basic authentication).
    /// </summary>
    /// <param name="wiremock">The <see cref="IResourceBuilder{WireMockServerResource}"/>.</param>
    /// <param name="username">The admin username.</param>
    /// <param name="password">The admin password.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> WithAdminUserNameAndPassword(this IResourceBuilder<WireMockServerResource> wiremock, string username, string password)
    {
        Guard.NotNull(wiremock);

        wiremock.Resource.Arguments.AdminUsername = Guard.NotNull(username);
        wiremock.Resource.Arguments.AdminPassword = Guard.NotNull(password);
        return wiremock;
    }
}