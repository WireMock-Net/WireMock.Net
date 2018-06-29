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
        /// The matchers.
        /// </summary>
        public IReadOnlyList<IStringMatcher> Matchers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="key">The key.</param>
        public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, [NotNull] string key) : this(matchBehaviour, key, (IStringMatcher[])null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, [NotNull] string key, [CanBeNull] string[] values) : this(matchBehaviour, key, values?.Select(value => new ExactMatcher(matchBehaviour, value)).Cast<IStringMatcher>().ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="key">The key.</param>
        /// <param name="matchers">The matchers.</param>
        public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, [NotNull] string key, [CanBeNull] IStringMatcher[] matchers)
        {
            Check.NotNull(key, nameof(key));

            _matchBehaviour = matchBehaviour;
            Key = key;
            Matchers = matchers;
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

            WireMockList<string> valuesPresentInRequestMessage = requestMessage.GetParameter(Key);
            if (valuesPresentInRequestMessage == null)
            {
                // Key is not present at all, just return Mismatch
                return MatchScores.Mismatch;
            }

            if (Matchers != null && Matchers.Any())
            {
                // Matchers are defined, just use the matchers to calculate the match score.
                var scores = new List<double>();
                foreach (string valuePresentInRequestMessage in valuesPresentInRequestMessage)
                {
                    double score = Matchers.Max(m => m.IsMatch(valuePresentInRequestMessage));
                    scores.Add(score);
                }

                return scores.Any() ? scores.Average() : MatchScores.Mismatch;
            }

            if (Matchers == null || !Matchers.Any())
            {
                // Matchers are null or not defined, and Key is present, just return Perfect.
                return MatchScores.Perfect;
            }

            return MatchScores.Mismatch;
        }
    }
}