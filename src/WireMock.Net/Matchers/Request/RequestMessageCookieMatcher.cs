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
    /// <inheritdoc cref="IRequestMatcher"/>
    public class RequestMessageCookieMatcher : IRequestMatcher
    {
        private readonly MatchBehaviour _matchBehaviour;
        private readonly bool _ignoreCase;

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
        public IStringMatcher[] Matchers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        public RequestMessageCookieMatcher(MatchBehaviour matchBehaviour, [NotNull] string name, [NotNull] string pattern, bool ignoreCase)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(pattern, nameof(pattern));

            _matchBehaviour = matchBehaviour;
            _ignoreCase = ignoreCase;
            Name = name;
            Matchers = new IStringMatcher[] { new WildcardMatcher(matchBehaviour, pattern, ignoreCase) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="name">The name.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        public RequestMessageCookieMatcher(MatchBehaviour matchBehaviour, [NotNull] string name, bool ignoreCase, [NotNull] params string[] patterns) :
            this(matchBehaviour, name, ignoreCase, patterns.Select(pattern => new WildcardMatcher(matchBehaviour, pattern, ignoreCase)).Cast<IStringMatcher>().ToArray())
        {
            Check.NotNull(patterns, nameof(patterns));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        public RequestMessageCookieMatcher(MatchBehaviour matchBehaviour, [NotNull] string name, bool ignoreCase, [NotNull] params IStringMatcher[] matchers)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(matchers, nameof(matchers));

            _matchBehaviour = matchBehaviour;
            Name = name;
            Matchers = matchers;
            _ignoreCase = ignoreCase;
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
        public double GetMatchingScore(IRequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            double score = IsMatch(requestMessage);
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch(IRequestMessage requestMessage)
        {
            if (requestMessage.Cookies == null)
            {
                return MatchBehaviourHelper.Convert(_matchBehaviour, MatchScores.Mismatch);
            }

            // Check if we want to use IgnoreCase to compare the Cookie-Name and Cookie-Value
            var cookies = !_ignoreCase ? requestMessage.Cookies : new Dictionary<string, string>(requestMessage.Cookies, StringComparer.OrdinalIgnoreCase);

            if (Funcs != null)
            {
                return MatchScores.ToScore(Funcs.Any(f => f(cookies)));
            }

            if (Matchers == null)
            {
                return MatchScores.Mismatch;
            }

            if (!cookies.ContainsKey(Name))
            {
                return MatchBehaviourHelper.Convert(_matchBehaviour, MatchScores.Mismatch);
            }

            string value = cookies[Name];
            return Matchers.Max(m => m.IsMatch(value));
        }
    }
}