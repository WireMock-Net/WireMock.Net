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
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        public RequestMessageHeaderMatcher([NotNull] string name, [NotNull] string pattern, bool ignoreCase = true)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(pattern, nameof(pattern));

            Name = name;
            Matchers = new IStringMatcher[] { new WildcardMatcher(pattern, ignoreCase) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageHeaderMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        public RequestMessageHeaderMatcher([NotNull] string name, [NotNull] string[] patterns, bool ignoreCase = true)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(patterns, nameof(patterns));

            Name = name;
            Matchers = patterns.Select(pattern => new WildcardMatcher(pattern, ignoreCase)).Cast<IStringMatcher>().ToArray();
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
                return MatchScores.Mismatch;
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
                return MatchScores.Mismatch;
            }

            WireMockList<string> list = requestMessage.Headers[Name];
            return Matchers.Max(m => list.Max(value => m.IsMatch(value))); // TODO : is this correct ?
        }
    }
}