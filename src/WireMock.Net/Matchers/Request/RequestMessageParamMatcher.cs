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
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public RequestMessageParamMatcher([NotNull] string key, [NotNull] IEnumerable<string> values)
        {
            Check.NotNull(key, nameof(key));
            Check.NotNull(values, nameof(values));

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
            requestMatchResult.MatchScore += score;

            requestMatchResult.Total++;

            return score;
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            if (Funcs != null)
                return MatchScores.ToScore(requestMessage.Query != null && Funcs.Any(f => f(requestMessage.Query)));

            List<string> values = requestMessage.GetParameter(Key);

            return MatchScores.ToScore(values?.Intersect(Values).Count() == Values.Count());
        }
    }
}