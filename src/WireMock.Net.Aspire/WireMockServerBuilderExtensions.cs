using Aspire.Hosting.ApplicationModel;
using Stef.Validation;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding WireMock.Net Server resources to the application model.
/// </summary>
public static class WireMockServerBuilderExtensions
{
    private const int DefaultContainerPort = 80;

    // https://github.com/dotnet/aspire/issues/854
    private const string DefaultLinuxImage = "sheyenrath/wiremock.net";
    private const string DefaultLinuxMappingsPath = "/app/__admin/mappings";


    /// <summary>
    /// Adds a SQL Server resource to the application model. A container is used for local development.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="port">The port for the WireMock Server.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> AddWireMock(this IDistributedApplicationBuilder builder, string name, int? port = null)
    {
        var args = new WireMockServerArguments();
        if (port > 0)
        {
            args.Port = port.Value;
        }

        return builder.AddWireMock(name, args);
    }

    public static IResourceBuilder<WireMockServerResource> AddWireMock(this IDistributedApplicationBuilder builder, string name, WireMockServerArguments arguments)
    {
        Guard.NotNull(arguments);

        var wireMockContainerResource = new WireMockServerResource(name);
        var resource = builder
            .AddResource(wireMockContainerResource)
            .WithHttpEndpoint(targetPort: DefaultContainerPort, port: arguments.Port)
            .WithImage(DefaultLinuxImage);

        if (!string.IsNullOrEmpty(arguments.MappingsPath))
        {
            resource = resource.WithBindMount(arguments.MappingsPath, DefaultLinuxMappingsPath);
        }

        var containerArgs = arguments.GetArgs();
        if (containerArgs.Length > 0)
        {
            resource = resource.WithArgs(containerArgs);
        }

        return resource;
    }
}