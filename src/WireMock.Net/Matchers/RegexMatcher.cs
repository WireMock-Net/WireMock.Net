using System;
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
        private readonly string _pattern;
        private readonly Regex _expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public RegexMatcher([NotNull, RegexPattern] string pattern, bool ignoreCase = false)
        {
            Check.NotNull(pattern, nameof(pattern));

            _pattern = pattern;

            RegexOptions options = RegexOptions.Compiled;
            if (ignoreCase)
                options |= RegexOptions.IgnoreCase;
            
            _expression = new Regex(_pattern, options);
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
                return MatchScores.ToScore(_expression.IsMatch(input));
            }
            catch (Exception)
            {
                return MatchScores.Mismatch;
            }
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <returns>Pattern</returns>
        public virtual string GetPattern()
        {
            return _pattern;
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