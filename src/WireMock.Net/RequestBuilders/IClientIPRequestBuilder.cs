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
        /// WithClientIP: add matching on ClientIP matchers.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithClientIP([NotNull] params IStringMatcher[] matchers);

        /// <summary>
        /// WithClientIP: add matching on clientIPs.
        /// </summary>
        /// <param name="clientIPs">The clientIPs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithClientIP([NotNull] params string[] clientIPs);

        /// <summary>
        /// WithClientIP: add matching on clientIPs and matchBehaviour.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="clientIPs">The clientIPs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithClientIP(MatchBehaviour matchBehaviour, [NotNull] params string[] clientIPs);

        /// <summary>
        /// WithClientIP: add matching on ClientIP funcs.
        /// </summary>
        /// <param name="funcs">The path funcs.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithClientIP([NotNull] params Func<string, bool>[] funcs);
    }
}