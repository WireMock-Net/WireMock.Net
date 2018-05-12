using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request parameters matcher.
    /// </summary>
    public class RequestMessageParamMatcher : IRequestMatcher
    {
        private readonly MatchBehaviour _matchBehaviour;

        /// <summary>
        /// The funcs
        /// </summary>
        public Func<IDictionary<string, WireMockList<string>>, bool>[] Funcs { get; }

        /// <summary>
        /// The key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The values
        /// </summary>
        public IEnumerable<string> Values { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="key">The key.</param>
        public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, [NotNull] string key) : this(matchBehaviour, key, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, [NotNull] string key, [CanBeNull] IEnumerable<string> values)
        {
            Check.NotNull(key, nameof(key));

            _matchBehaviour = matchBehaviour;
            Key = key;
            Values = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        public RequestMessageParamMatcher([NotNull] params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs)
        {
            Check.NotNull(funcs, nameof(funcs));

            Funcs = funcs;
        }

        /// <inheritdoc cref="IRequestMatcher.GetMatchingScore"/>
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            double score = MatchBehaviourHelper.Convert(_matchBehaviour, IsMatch(requestMessage));
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            if (Funcs != null)
            {
                return MatchScores.ToScore(requestMessage.Query != null && Funcs.Any(f => f(requestMessage.Query)));
            }

            var values = requestMessage.GetParameter(Key);
            if (values == null)
            {
                // Key is not present, just return Mismatch
                return MatchScores.Mismatch;
            }

            if (values.Count == 0 && (Values == null || !Values.Any()))
            {
                // Key is present, but no values or null, just return Perfect
                return MatchScores.Perfect;
            }

            var matches = Values.Select(v => values.Contains(v));
            return MatchScores.ToScore(matches);
        }
    }
}