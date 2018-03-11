using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Util;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The ParamsRequestBuilder interface.
    /// </summary>
    public interface IParamsRequestBuilder
    {
        /// <summary>
        /// WithParam (key only)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] string key);

        /// <summary>
        /// WithParam (values)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] string key, [CanBeNull] params string[] values);

        /// <summary>
        /// WithParam (funcs)
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithParam([NotNull] params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs);
    }
}