using System;
using System.Collections.Generic;
using JetBrains.Annotations;

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
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithParam([NotNull] string key, params string[] values);

        /// <summary>
        /// The with parameters.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithParam([NotNull] Func<IDictionary<string, List<string>>, bool> func);
    }
}