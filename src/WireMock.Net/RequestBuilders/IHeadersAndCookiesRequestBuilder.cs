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
        /// WithHeader: matching based on name, pattern and matchBehaviour.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] string name, string pattern, MatchBehaviour matchBehaviour);

        /// <summary>
        /// WithHeader: matching based on name, pattern, ignoreCase and matchBehaviour.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] string name, string pattern, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// WithHeader: matching based on name, patterns and matchBehaviour.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] string name, string[] patterns, MatchBehaviour matchBehaviour);

        /// <summary>
        /// WithHeader: matching based on name, patterns, ignoreCase and matchBehaviour.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] string name, string[] patterns, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// WithHeader: matching based on name and IStringMatcher[].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] string name, [NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// WithHeader: matching based on functions.
        /// </summary>
        /// <param name="funcs">The headers funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithHeader([NotNull] params Func<IDictionary<string, string[]>, bool>[] funcs);

        /// <summary>
        /// WithCookie: cookie matching based on name, pattern, ignoreCase and matchBehaviour.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithCookie([NotNull] string name, string pattern, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// WithCookie: matching based on name and IStringMatcher[].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithCookie([NotNull] string name, [NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// WithCookie: matching based on functions.
        /// </summary>
        /// <param name="cookieFuncs">The funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithCookie([NotNull] params Func<IDictionary<string, string>, bool>[] cookieFuncs);
    }
}