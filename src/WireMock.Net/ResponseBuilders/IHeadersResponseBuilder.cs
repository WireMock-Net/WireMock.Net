using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The HeadersResponseBuilder interface.
    /// </summary>
    public interface IHeadersResponseBuilder : IBodyResponseBuilder
    {
        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithHeader([NotNull] string name, string value);

        /// <summary>
        /// The with headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithHeaders([NotNull] IDictionary<string,string> headers);
    }
}