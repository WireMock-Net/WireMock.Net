using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The HeadersAndCookieRequestBuilder interface.
    /// </summary>
    public interface IHeadersAndCookiesRequestBuilder : IBodyRequestBuilder, IRequestMatcher, IParamsRequestBuilder
    {
        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">ignore Case</param>
        /// <returns>The <see cref="IHeadersAndCookiesRequestBuilder"/>.</returns>
        IHeadersAndCookiesRequestBuilder WithHeader(string name, string pattern, bool ignoreCase = true);

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="func">The headers func.</param>
        /// <returns>The <see cref="IHeadersAndCookiesRequestBuilder"/>.</returns>
        IHeadersAndCookiesRequestBuilder WithHeader([NotNull] params Func<IDictionary<string, string>, bool>[] func);

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">ignore Case</param>
        /// <returns>The <see cref="IHeadersAndCookiesRequestBuilder"/>.</returns>
        IHeadersAndCookiesRequestBuilder WithCookie(string name, string pattern, bool ignoreCase = true);

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="cookieFunc">The func.</param>
        /// <returns>The <see cref="IHeadersAndCookiesRequestBuilder"/>.</returns>
        IHeadersAndCookiesRequestBuilder WithCookie([NotNull] params Func<IDictionary<string, string>, bool>[] cookieFunc);
    }
}