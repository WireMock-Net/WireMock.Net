using Stef.Validation;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting.ApplicationModel;

/// <summary>
/// A resource that represents a WireMock.Net Server.
/// </summary>
public class WireMockServerResource : ContainerResource, IResourceWithServiceDiscovery
{
    private const string PrimaryEndpointName = "http";
    private EndpointReference? _primaryEndpoint;

    internal WireMockServerArguments Arguments { get; }

    /// <summary>
    /// Gets the primary endpoint for the server.
    /// </summary>
    public EndpointReference PrimaryEndpoint => _primaryEndpoint ??= new(this, PrimaryEndpointName);

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockServerResource"/> class.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="arguments">The arguments to start the WireMock.Net Server.</param>
    public WireMockServerResource(string name, WireMockServerArguments arguments) : base(name)
    {
        Arguments = Guard.NotNull(arguments);
    }
}