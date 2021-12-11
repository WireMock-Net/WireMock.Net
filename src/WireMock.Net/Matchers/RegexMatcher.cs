using System;
using System.Linq;
using System.Text.RegularExpressions;
using AnyOfTypes;
using JetBrains.Annotations;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.RegularExpressions;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// Regular Expression Matcher
    /// </summary>
    /// <inheritdoc cref="IStringMatcher"/>
    /// <inheritdoc cref="IIgnoreCaseMatcher"/>
    public class RegexMatcher : IStringMatcher, IIgnoreCaseMatcher
    {
        private readonly AnyOf<string, StringPattern>[] _patterns;
        private readonly RegexExtended[] _expressions;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        public RegexMatcher([NotNull, RegexPattern] AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(new[] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        public RegexMatcher(MatchBehaviour matchBehaviour, [NotNull, RegexPattern] AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(matchBehaviour, new[] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        public RegexMatcher([NotNull, RegexPattern] AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, patterns, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        public RegexMatcher(MatchBehaviour matchBehaviour, [NotNull, RegexPattern] AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false, bool throwException = false)
        {
            Check.NotNull(patterns, nameof(patterns));

            _patterns = patterns;
            IgnoreCase = ignoreCase;
            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;

            RegexOptions options = RegexOptions.Compiled | RegexOptions.Multiline;

            if (ignoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }

            _expressions = patterns.Select(p => new RegexExtended(p.GetPattern(), options)).ToArray();
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public virtual double IsMatch(string input)
        {
            double match = MatchScores.Mismatch;
            if (input != null)
            {
                try
                {
                    match = MatchScores.ToScore(_expressions.Select(e => e.IsMatch(input)));
                }
                catch (Exception)
                {
                    if (ThrowException)
                    {
                        throw;
                    }
                }
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public virtual AnyOf<string, StringPattern>[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public virtual string Name => "RegexMatcher";

        /// <inheritdoc cref="IIgnoreCaseMatcher.IgnoreCase"/>
        public bool IgnoreCase { get; }

        /// <summary>
        /// Use the <see cref="RegexExtended"/> instead of the default <see cref="Regex"/>.
        /// </summary>
        public bool Extended { get; } = true;
    }
}