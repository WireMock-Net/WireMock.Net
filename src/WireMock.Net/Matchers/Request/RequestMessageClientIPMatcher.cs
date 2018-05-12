using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request ClientIP matcher.
    /// </summary>
    public class RequestMessageClientIPMatcher : IRequestMatcher
    {
        /// <summary>
        /// The matchers.
        /// </summary>
        public IReadOnlyList<IStringMatcher> Matchers { get; }

        /// <summary>
        /// The ClientIP functions.
        /// </summary>
        public Func<string, bool>[] Funcs { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
        /// </summary>
        /// <param name="clientIPs">The clientIPs.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        public RequestMessageClientIPMatcher(MatchBehaviour matchBehaviour, [NotNull] params string[] clientIPs) : this(clientIPs.Select(ip => new WildcardMatcher(matchBehaviour, ip)).Cast<IStringMatcher>().ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        public RequestMessageClientIPMatcher([NotNull] params IStringMatcher[] matchers)
        {
            Check.NotNull(matchers, nameof(matchers));
            Matchers = matchers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The clientIP functions.</param>
        public RequestMessageClientIPMatcher([NotNull] params Func<string, bool>[] funcs)
        {
            Check.NotNull(funcs, nameof(funcs));
            Funcs = funcs;
        }

        /// <inheritdoc cref="IRequestMatcher.GetMatchingScore"/>
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            double score = IsMatch(requestMessage);
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            if (Matchers != null)
            {
                return Matchers.Max(matcher => matcher.IsMatch(requestMessage.ClientIP));
            }

            if (Funcs != null)
            {
                return MatchScores.ToScore(requestMessage.ClientIP != null && Funcs.Any(func => func(requestMessage.ClientIP)));
            }

            return MatchScores.Mismatch;
        }
    }
}