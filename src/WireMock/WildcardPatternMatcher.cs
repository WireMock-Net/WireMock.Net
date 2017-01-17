using System.Diagnostics.CodeAnalysis;

[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]

namespace WireMock
{
    /// <summary>
    /// The wildcard pattern matcher.
    /// </summary>
    public static class WildcardPatternMatcher
    {
        /// <summary>
        /// The match wildcard string.
        /// </summary>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <remarks>
        /// Copy/paste from http://www.codeproject.com/Tips/57304/Use-wildcard-characters-and-to-compare-strings
        /// </remarks>
        public static bool MatchWildcardString(string pattern, string input)
        {
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
                return MatchWildcardString(pattern.Substring(1), input) || MatchWildcardString(pattern, input.Substring(1));
            }

            if (pattern[pattern.Length - 1] == '*')
            {
                return MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input) || MatchWildcardString(pattern, input.Substring(0, input.Length - 1));
            }

            return pattern[0] == input[0] && MatchWildcardString(pattern.Substring(1), input.Substring(1));
        }
    }
}
