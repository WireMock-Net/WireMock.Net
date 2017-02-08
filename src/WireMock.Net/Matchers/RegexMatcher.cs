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
    /// <seealso cref="IMatcher" />
    public class RegexMatcher : IMatcher
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
                options |= RegexOptions.IgnoreCase;

            _expressions = patterns.Select(p => new Regex(p, options)).ToArray();
        }

        /// <summary>
        /// Determines whether the specified input is match.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
        public double IsMatch(string input)
        {
            if (input == null)
                return MatchScores.Mismatch;

            try
            {
                return MatchScores.ToScore(_expressions.Select(e => e.IsMatch(input)));
            }
            catch (Exception)
            {
                return MatchScores.Mismatch;
            }
        }

        /// <summary>
        /// Gets the patterns.
        /// </summary>
        /// <returns>Pattern</returns>
        public virtual string[] GetPatterns()
        {
            return _patterns;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>
        /// Name
        /// </returns>
        public virtual string GetName()
        {
            return "RegexMatcher";
        }
    }
}