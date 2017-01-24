using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// WildcardMatcher
    /// </summary>
    /// <seealso cref="WireMock.Matchers.IMatcher" />
    public class WildcardMatcher : IMatcher
    {
        private readonly string _pattern;
        private readonly bool _ignoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public WildcardMatcher([NotNull] string pattern, bool ignoreCase = false)
        {
            Check.NotNull(pattern, nameof(pattern));

            _pattern = pattern;
            _ignoreCase = ignoreCase;
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
            return MatchWildcardString(_pattern, input);
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <returns>Pattern</returns>
        public string GetPattern()
        {
            return _pattern;
        }

        /// <summary>
        /// Copy/paste from http://www.codeproject.com/Tips/57304/Use-wildcard-characters-and-to-compare-strings
        /// </summary>
        private bool MatchWildcardString([NotNull] string pattern, string input)
        {
            if (input != null && _ignoreCase)
                input = input.ToLower();

            if (_ignoreCase)
                pattern = pattern.ToLower();

            if (string.CompareOrdinal(pattern, input) == 0)
            {
                return true;
            }

            if (string.IsNullOrEmpty(input))
            {
                return string.IsNullOrEmpty(pattern.Trim('*'));
            }

            if (pattern.Length == 0)
            {
                return false;
            }

            if (pattern[0] == '?')
            {
                return MatchWildcardString(pattern.Substring(1), input.Substring(1));
            }

            if (pattern[pattern.Length - 1] == '?')
            {
                return MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input.Substring(0, input.Length - 1));
            }

            if (pattern[0] == '*')
            {
                if (MatchWildcardString(pattern.Substring(1), input))
                {
                    return true;
                }
                return MatchWildcardString(pattern, input.Substring(1));
            }

            if (pattern[pattern.Length - 1] == '*')
            {
                if (MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input))
                {
                    return true;
                }
                return MatchWildcardString(pattern, input.Substring(0, input.Length - 1));
            }

            if (pattern[0] == input[0])
            {
                return MatchWildcardString(pattern.Substring(1), input.Substring(1));
            }

            return false;
        }
    }
}