using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request path matcher.
    /// </summary>
    public class RequestMessagePathMatcher : IRequestMatcher
    {
        /// <summary>
        /// The matcher.
        /// </summary>
        public IReadOnlyList<IMatcher> Matchers { get; }

        /// <summary>
        /// The path functions
        /// </summary>
        public Func<string, bool>[] Funcs { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        public RequestMessagePathMatcher([NotNull] params string[] paths) : this(paths.Select(path => new WildcardMatcher(path)).ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        public RequestMessagePathMatcher([NotNull] params IMatcher[] matchers)
        {
            Check.NotNull(matchers, nameof(matchers));
            Matchers = matchers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The path functions.</param>
        public RequestMessagePathMatcher([NotNull] params Func<string, bool>[] funcs)
        {
            Check.NotNull(funcs, nameof(funcs));
            Funcs = funcs;
        }

        /// <summary>
        /// Determines whether the specified RequestMessage is match.
        /// </summary>
        /// <param name="requestMessage">The RequestMessage.</param>
        /// <param name="requestMatchResult">The RequestMatchResult.</param>
        /// <returns>
        /// A value between 0.0 - 1.0 of the similarity.
        /// </returns>
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            double score = IsMatch(requestMessage);
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            if (Matchers != null)
                return Matchers.Max(m => m.IsMatch(requestMessage.Path));

            if (Funcs != null)
                return MatchScores.ToScore(requestMessage.Path != null && Funcs.Any(func => func(requestMessage.Path)));

            return MatchScores.Mismatch;
        }
    }
}