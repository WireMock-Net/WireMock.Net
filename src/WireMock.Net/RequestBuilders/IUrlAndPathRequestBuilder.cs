﻿using System;
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
        /// The with path.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithPath([NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithPath([NotNull] params string[] paths);

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="funcs">The path funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithPath([NotNull] params Func<string, bool>[] funcs);

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithUrl([NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithUrl([NotNull] params string[] urls);

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="func">The path func.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithUrl([NotNull] params Func<string, bool>[] func);
    }
}