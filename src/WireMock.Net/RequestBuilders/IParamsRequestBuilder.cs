using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] string key, [CanBeNull] params string[] values);

        /// <summary>
        /// The with parameters.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs);
    }
}