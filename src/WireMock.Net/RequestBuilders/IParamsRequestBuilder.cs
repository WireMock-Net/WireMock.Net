using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Matchers;
using WireMock.Util;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The ParamsRequestBuilder interface.
    /// </summary>
    public interface IParamsRequestBuilder
    {
        /// <summary>
        /// WithParam: matching on key only.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="matchBehaviour">The match behaviour (optional).</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] string key, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// WithParam: matching on key and values.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] string key, [CanBeNull] params string[] values);

        /// <summary>
        /// WithParam: matching on key, values and matchBehaviour.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] string key, MatchBehaviour matchBehaviour, [CanBeNull] params string[] values);

        /// <summary>
        /// WithParam: matching on functions.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs);
    }
}