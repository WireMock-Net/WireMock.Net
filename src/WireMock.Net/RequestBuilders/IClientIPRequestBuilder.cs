using System;
using JetBrains.Annotations;
using WireMock.Matchers;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The IClientIPRequestBuilder interface.
    /// </summary>
    public interface IClientIPRequestBuilder : IUrlAndPathRequestBuilder
    {
        /// <summary>
        /// The with ClientIP.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithClientIP([NotNull] params IMatcher[] matchers);

        /// <summary>
        /// The with ClientIP.
        /// </summary>
        /// <param name="clientIPs">The clientIPs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithClientIP([NotNull] params string[] clientIPs);

        /// <summary>
        /// The with ClientIP.
        /// </summary>
        /// <param name="funcs">The path funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithClientIP([NotNull] params Func<string, bool>[] funcs);
    }
}