using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request cookie matcher.
    /// </summary>
    public class RequestMessageCookieMatcher : IRequestMatcher
    {
        private readonly MatchBehaviour _matchBehaviour;

        /// <value>
        /// The funcs.
        /// </value>
        public Func<IDictionary<string, string>, bool>[] Funcs { get; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; }

        /// <value>
        /// The matchers.
        /// </value>
        public IStringMatcher[] Matchers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">The ignoreCase.</param>
        public RequestMessageCookieMatcher(MatchBehaviour matchBehaviour, [NotNull] string name, [NotNull] string pattern, bool ignoreCase = true)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(pattern, nameof(pattern));

            _matchBehaviour = matchBehaviour;
            Name = name;
            Matchers = new IStringMatcher[] { new WildcardMatcher(matchBehaviour, pattern, ignoreCase) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        public RequestMessageCookieMatcher([NotNull] string name, [NotNull] params IStringMatcher[] matchers)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(matchers, nameof(matchers));

            Name = name;
            Matchers = matchers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        public RequestMessageCookieMatcher([NotNull] params Func<IDictionary<string, string>, bool>[] funcs)
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
            if (requestMessage.Cookies == null)
            {
                return MatchBehaviourHelper.Convert(_matchBehaviour, MatchScores.Mismatch);
            }

            if (Funcs != null)
            {
                return MatchScores.ToScore(Funcs.Any(f => f(requestMessage.Cookies)));
            }

            if (Matchers == null)
            {
                return MatchScores.Mismatch;
            }

            if (!requestMessage.Cookies.ContainsKey(Name))
            {
                return MatchBehaviourHelper.Convert(_matchBehaviour, MatchScores.Mismatch);
            }

            string value = requestMessage.Cookies[Name];
            return Matchers.Max(m => m.IsMatch(value));
        }
    }
}