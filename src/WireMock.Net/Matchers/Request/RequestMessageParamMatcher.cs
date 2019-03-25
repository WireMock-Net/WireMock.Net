using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Defines if the key should be matched using case-ignore.
        /// </summary>
        public bool? IgnoreCase { get; private set; }

        /// <summary>
        /// The matchers.
        /// </summary>
        public IReadOnlyList<IStringMatcher> Matchers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="key">The key.</param>
        /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
        public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, [NotNull] string key, bool ignoreCase) : this(matchBehaviour, key, ignoreCase, (IStringMatcher[])null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="key">The key.</param>
        /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
        /// <param name="values">The values.</param>
        public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, [NotNull] string key, bool ignoreCase, [CanBeNull] string[] values) : this(matchBehaviour, key, ignoreCase, values?.Select(value => new ExactMatcher(matchBehaviour, value)).Cast<IStringMatcher>().ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="key">The key.</param>
        /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
        /// <param name="matchers">The matchers.</param>
        public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, [NotNull] string key, bool ignoreCase, [CanBeNull] IStringMatcher[] matchers)
        {
            Check.NotNull(key, nameof(key));

            _matchBehaviour = matchBehaviour;
            Key = key;
            IgnoreCase = ignoreCase;
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

            WireMockList<string> valuesPresentInRequestMessage = requestMessage.GetParameter(Key, IgnoreCase ?? false);
            if (valuesPresentInRequestMessage == null)
            {
                // Key is not present at all, just return Mismatch
                return MatchScores.Mismatch;
            }

            if (Matchers != null && Matchers.Any())
            {
                // Return the score based on Matchers and valuesPresentInRequestMessage
                return CalculateScore(valuesPresentInRequestMessage);
            }

            if (Matchers == null || !Matchers.Any())
            {
                // Matchers are null or not defined, and Key is present, just return Perfect.
                return MatchScores.Perfect;
            }

            return MatchScores.Mismatch;
        }

        private double CalculateScore(WireMockList<string> valuesPresentInRequestMessage)
        {
            var total = new List<double>();

            // If the total patterns in all matchers > values in message, use the matcher as base
            if (Matchers.Sum(m => m.GetPatterns().Length) > valuesPresentInRequestMessage.Count)
            {
                foreach (var matcher in Matchers)
                {
                    double score = 0d;
                    foreach (string valuePresentInRequestMessage in valuesPresentInRequestMessage)
                    {
                        score += matcher.IsMatch(valuePresentInRequestMessage) / matcher.GetPatterns().Length;
                    }

                    total.Add(score);
                }
            }
            else
            {
                foreach (string valuePresentInRequestMessage in valuesPresentInRequestMessage)
                {
                    double score = Matchers.Max(m => m.IsMatch(valuePresentInRequestMessage));
                    total.Add(score);
                }
            }

            return total.Any() ? MatchScores.ToScore(total) : MatchScores.Mismatch;
        }
    }
}