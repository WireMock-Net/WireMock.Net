using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The HeadersAndCookieRequestBuilder interface.
    /// </summary>
    public interface IHeadersAndCookiesRequestBuilder : IBodyRequestBuilder, IRequestMatcher, IParamsRequestBuilder
    {
        /// <summary>
        /// Add Header matching based on name, pattern and ignoreCase.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">ignore Case</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] string name, string pattern, bool ignoreCase = true);

        /// <summary>
        /// Add Header matching based on name, patterns and ignoreCase.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">ignore Case</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] string name, string[] patterns, bool ignoreCase = true);

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] string name, [NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="funcs">The headers funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] params Func<IDictionary<string, string[]>, bool>[] funcs);

        /// <summary>
        /// The with cookie.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">ignore Case</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithCookie([NotNull] string name, string pattern, bool ignoreCase = true);

        /// <summary>
        /// The with cookie.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithCookie([NotNull] string name, [NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// The with cookie.
        /// </summary>
        /// <param name="cookieFuncs">The funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithCookie([NotNull] params Func<IDictionary<string, string>, bool>[] cookieFuncs);
    }
}