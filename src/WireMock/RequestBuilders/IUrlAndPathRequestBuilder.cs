using System;
using JetBrains.Annotations;
using WireMock.Matchers;

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
        /// <param name="matcher">The matcher.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithUrl([NotNull] IMatcher matcher);

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithUrl([NotNull] params string[] url);

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="func">The url func.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithUrl([NotNull] params Func<string, bool>[] func);

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithPath([NotNull] IMatcher matcher);

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithPath([NotNull] params string[] path);

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="func">The path func.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        IUrlAndPathRequestBuilder WithPath([NotNull] params Func<string, bool>[] func);
    }
}