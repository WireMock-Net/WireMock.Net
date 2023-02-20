using System.Collections.Generic;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The ResponseBuilder interface.
/// </summary>
public interface IResponseBuilder : IProxyResponseBuilder
{
    IResponseBuilder WithData(IDictionary<string, object?> data);
}