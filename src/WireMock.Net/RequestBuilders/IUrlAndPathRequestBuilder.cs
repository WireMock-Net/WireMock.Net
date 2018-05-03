using System;
using JetBrains.Annotations;
using WireMock.Matchers;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// IUrlAndPathRequestBuilder
    /// </summary>
    public interface IUrlAndPathRequestBuilder : IMethodRequestBuilder
    {
        /// <summary>
        /// WithPath: add path matching based on IStringMatchers.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithPath([NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// WithPath: add path matching based on paths.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithPath([NotNull] params string[] paths);

        /// <summary>
        /// WithPath: add path matching based on paths and matchBehaviour.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="paths">The paths.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithPath(MatchBehaviour matchBehaviour, [NotNull] params string[] paths);

        /// <summary>
        /// WithPath: add path matching based on functions.
        /// </summary>
        /// <param name="funcs">The path funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithPath([NotNull] params Func<string, bool>[] funcs);

        /// <summary>
        /// WithUrl: add url matching based on IStringMatcher[].
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithUrl([NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// WithUrl: add url matching based on urls.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithUrl([NotNull] params string[] urls);

        /// <summary>
        /// WithUrl: add url matching based on urls.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="urls">The urls.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithUrl(MatchBehaviour matchBehaviour, [NotNull] params string[] urls);

        /// <summary>
        /// WithUrl: add url matching based on functions.
        /// </summary>
        /// <param name="func">The path func.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithUrl([NotNull] params Func<string, bool>[] func);
    }
}