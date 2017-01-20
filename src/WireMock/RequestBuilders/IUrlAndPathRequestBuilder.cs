using System;
using JetBrains.Annotations;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// IUrlAndPathRequestBuilder
    /// </summary>
    public interface IUrlAndPathRequestBuilder : IVerbRequestBuilder
    {
        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithUrl([NotNull] string url);

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="func">The url func.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithUrl([NotNull] Func<string, bool> func);

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithPath([NotNull] string path);

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="func">The path func.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithPath([NotNull] Func<string, bool> func);
    }
}