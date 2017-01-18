using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The HeadersRequestBuilder interface.
    /// </summary>
    public interface IHeadersRequestBuilder : IBodyRequestBuilder, ISpecifyRequests, IParamsRequestBuilder
    {
        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="ignoreCase">ignore Case</param>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder WithHeader(string name, string value, bool ignoreCase = true);

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="func">
        ///     The headers func.
        /// </param>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder WithHeader([NotNull] Func<IDictionary<string, string>, bool> func);
    }
}