using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting.ApplicationModel;

/// <summary>
/// A resource that represents a SQL Server container.
/// </summary>
public class WireMockServerResource : ContainerResource, IResourceWithServiceDiscovery
{
    // internal const string PrimaryEndpointName = "http";

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockServerResource"/> class.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    public WireMockServerResource(string name) : base(name)
    {
        // PrimaryEndpoint = new EndpointReference(this, PrimaryEndpointName);
    }

    //public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{GetConnectionString()}");

    //public string GetConnectionString()
    //{
    //    var endpoint = this.GetEndpoints().FirstOrDefault(e => e.IsAllocated);
    //    if (endpoint == null)
    //    {
    //        throw new InvalidOperationException("No endpoints are allocated.");
    //    }

    //    return $"{endpoint.Host}:{endpoint.Port}";
    //}

    /// <summary>
    /// Gets the primary endpoint for the WireMock.Net Server.
    /// </summary>
    //public EndpointReference PrimaryEndpoint { get; }

    //private ReferenceExpression ConnectionString =>
    //    ReferenceExpression.Create(
    //        $"{PrimaryEndpoint.Property(EndpointProperty.Host)}:{PrimaryEndpoint.Property(EndpointProperty.Port)}");
    
    //public ReferenceExpression ConnectionStringExpression
    //{
    //    get
    //    {
    //        if (this.TryGetLastAnnotation<ConnectionStringRedirectAnnotation>(out var connectionStringAnnotation))
    //        {
    //            return connectionStringAnnotation.Resource.ConnectionStringExpression;
    //        }

    //        return ConnectionString;

    //        // Hack
    //        //var url = "http://" + PrimaryEndpoint.Host + ":" + PrimaryEndpoint.Port;
    //        //return ReferenceExpression.Create($"{url}");
    //    }
    //}
}