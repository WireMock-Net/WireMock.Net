using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request path matcher.
    /// </summary>
    public class RequestMessagePathMatcher : IRequestMatcher
    {
        /// <summary>
        /// The matchers
        /// </summary>
        public IReadOnlyList<IStringMatcher>? Matchers { get; }

        /// <summary>
        /// The path functions
        /// </summary>
        public Func<string, bool>[]? Funcs { get; }

        /// <summary>
        /// The <see cref="MatchBehaviour"/>
        /// </summary>
        public MatchBehaviour Behaviour { get; }

        /// <summary>
        /// The <see cref="MatchOperator"/>
        /// </summary>
        public MatchOperator Operator { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
        /// <param name="paths">The paths.</param>
        public RequestMessagePathMatcher(
            MatchBehaviour matchBehaviour,
            MatchOperator matchOperator,
            params string[] paths) :
            this(matchBehaviour, matchOperator, paths
                .Select(path => new WildcardMatcher(matchBehaviour, new AnyOf<string, StringPattern>[] { path }, false, false, matchOperator))
                .Cast<IStringMatcher>().ToArray())
        {
            Operator = matchOperator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
        /// <param name="matchers">The matchers.</param>
        public RequestMessagePathMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params IStringMatcher[] matchers)
        {
            Matchers = Guard.NotNull(matchers);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
        /// <param name="funcs">The path functions.</param>
        public RequestMessagePathMatcher(params Func<string, bool>[] funcs)
        {
            Funcs = Guard.NotNull(funcs);
        }

        /// <inheritdoc cref="IRequestMatcher.GetMatchingScore"/>
        public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
        {
            double score = IsMatch(requestMessage);
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch(IRequestMessage requestMessage)
        {
            if (Matchers != null)
            {
                return Matchers.Max(m => m.IsMatch(requestMessage.Path));
            }

            if (Funcs != null)
            {
                return MatchScores.ToScore(Behaviour, requestMessage.Path != null && Funcs.Any(func => func(requestMessage.Path)));
            }

            return MatchScores.Mismatch;
        }
    }
}