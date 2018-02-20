using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// Regular Expression Matcher
    /// </summary>
    /// <seealso cref="IStringMatcher" />
    public class RegexMatcher : IStringMatcher
    {
        private readonly string[] _patterns;
        private readonly Regex[] _expressions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public RegexMatcher([NotNull, RegexPattern] string pattern, bool ignoreCase = false) : this(new [] { pattern }, ignoreCase )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public RegexMatcher([NotNull, RegexPattern] string[] patterns, bool ignoreCase = false)
        {
            Check.NotNull(patterns, nameof(patterns));

            _patterns = patterns;

            RegexOptions options = RegexOptions.Compiled;
            if (ignoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }

            _expressions = patterns.Select(p => new Regex(p, options)).ToArray();
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            if (input == null)
            {
                return MatchScores.Mismatch;
            }

            try
            {
                return MatchScores.ToScore(_expressions.Select(e => e.IsMatch(input)));
            }
            catch (Exception)
            {
                return MatchScores.Mismatch;
            }
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public virtual string[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.GetName"/>
        public virtual string GetName()
        {
            return "RegexMatcher";
        }
    }
}