using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// Regular Expression Matcher
    /// </summary>
    /// <seealso cref="WireMock.Matchers.IMatcher" />
    public class RegexMatcher : IMatcher
    {
        private readonly Regex _expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public RegexMatcher([NotNull, RegexPattern] string pattern)
        {
            Check.NotNull(pattern, nameof(pattern));

            _expression = new Regex(pattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Determines whether the specified input is match.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if the specified input is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(string input)
        {
            if (input == null)
                return false;

            try
            {
                return _expression.IsMatch(input);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}