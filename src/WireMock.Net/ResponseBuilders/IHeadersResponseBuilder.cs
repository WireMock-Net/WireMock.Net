using System.Collections.Generic;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The HeadersResponseBuilder interface.
/// </summary>
public interface IHeadersResponseBuilder : IBodyResponseBuilder
{
    /// <summary>
    /// The with header.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="values">The values.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithHeader(string name, params string[] values);

    /// <summary>
    /// The with headers.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithHeaders(IDictionary<string, string> headers);

    /// <summary>
    /// The with headers.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithHeaders(IDictionary<string, string[]> headers);

    /// <summary>
    /// The with headers.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithHeaders(IDictionary<string, WireMockList<string>> headers);
}