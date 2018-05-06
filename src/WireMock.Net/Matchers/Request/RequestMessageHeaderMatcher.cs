using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request header matcher.
    /// </summary>
    /// <inheritdoc cref="IRequestMatcher"/>
    public class RequestMessageHeaderMatcher : IRequestMatcher
    {
        private readonly MatchBehaviour _matchBehaviour;

        /// <summary>
        /// The functions
        /// </summary>
        public Func<IDictionary<string, string[]>, bool>[] Funcs { get; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; }

        /// <value>
        /// The matchers.
        /// </value>
        public IStringMatcher[] Matchers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageHeaderMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        public RequestMessageHeaderMatcher(MatchBehaviour matchBehaviour, [NotNull] string name, [NotNull] string pattern, bool ignoreCase)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(pattern, nameof(pattern));

            _matchBehaviour = matchBehaviour;
            Name = name;
            Matchers = new IStringMatcher[] { new WildcardMatcher(matchBehaviour, pattern, ignoreCase) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageHeaderMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        /// <param name="matchBehaviour">The match behaviour.</param>
        public RequestMessageHeaderMatcher(MatchBehaviour matchBehaviour, [NotNull] string name, [NotNull] string[] patterns, bool ignoreCase)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(patterns, nameof(patterns));

            _matchBehaviour = matchBehaviour;
            Name = name;
            Matchers = patterns.Select(pattern => new WildcardMatcher(matchBehaviour, pattern, ignoreCase)).Cast<IStringMatcher>().ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageHeaderMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        public RequestMessageHeaderMatcher([NotNull] string name, [NotNull] params IStringMatcher[] matchers)
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
        public RequestMessageHeaderMatcher([NotNull] params Func<IDictionary<string, string[]>, bool>[] funcs)
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
            if (requestMessage.Headers == null)
            {
                return MatchBehaviourHelper.Convert(_matchBehaviour, MatchScores.Mismatch);
            }

            if (Funcs != null)
            {
                return MatchScores.ToScore(Funcs.Any(f => f(requestMessage.Headers.ToDictionary(entry => entry.Key, entry => entry.Value.ToArray()))));
            }

            if (Matchers == null)
            {
                return MatchScores.Mismatch;
            }

            if (!requestMessage.Headers.ContainsKey(Name))
            {
                return MatchBehaviourHelper.Convert(_matchBehaviour, MatchScores.Mismatch);
            }

            WireMockList<string> list = requestMessage.Headers[Name];
            return Matchers.Max(m => list.Max(value => m.IsMatch(value))); // TODO : is this correct ?
        }
    }
}