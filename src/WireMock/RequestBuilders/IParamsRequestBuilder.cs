using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Matchers.Request;
using WireMock.Util;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The ParametersRequestBuilder interface.
    /// </summary>
    public interface IParamsRequestBuilder
    {
        /// <summary>
        /// The with parameters.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="IRequestMatcher"/>.
        /// </returns>
        IRequestMatcher WithParam([NotNull] string key, params string[] values);

        /// <summary>
        /// The with parameters.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        /// <returns>The <see cref="IRequestMatcher"/>.</returns>
        IRequestMatcher WithParam([NotNull] params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs);
    }
}