using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request header matcher.
    /// </summary>
    public class RequestMessageHeaderMatcher : IRequestMatcher
    {
        /// <summary>
        /// The functions
        /// </summary>
        public Func<IDictionary<string, string>, bool>[] Funcs { get; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; }

        /// <value>
        /// The matchers.
        /// </value>
        public IMatcher[] Matchers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageHeaderMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">The ignoreCase.</param>
        public RequestMessageHeaderMatcher([NotNull] string name, [NotNull] string pattern, bool ignoreCase = true)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(pattern, nameof(pattern));

            Name = name;
            Matchers = new IMatcher[] { new WildcardMatcher(pattern, ignoreCase) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageHeaderMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        public RequestMessageHeaderMatcher([NotNull] string name, [NotNull] params IMatcher[] matchers)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(matchers, nameof(matchers));

            Name = name;
            Matchers = matchers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageHeaderMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        public RequestMessageHeaderMatcher([NotNull] params Func<IDictionary<string, string>, bool>[] funcs)
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
            requestMatchResult.TotalScore += score;

            requestMatchResult.TotalNumber++;

            return score;
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            if (requestMessage.Headers == null)
                return MatchScores.Mismatch;

            if (Funcs != null)
                return MatchScores.ToScore(Funcs.Any(f => f(requestMessage.Headers)));

            if (Matchers == null)
                return MatchScores.Mismatch;

            if (!requestMessage.Headers.ContainsKey(Name))
                return MatchScores.Mismatch;

            string value = requestMessage.Headers[Name];
            return Matchers.Max(m => m.IsMatch(value));
        }
    }
}