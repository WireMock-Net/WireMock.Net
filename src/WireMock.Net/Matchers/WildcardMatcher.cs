using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace WireMock.Matchers
{
    /// <summary>
    /// WildcardMatcher
    /// </summary>
    /// <seealso cref="IMatcher" />
    public class WildcardMatcher : RegexMatcher
    {
        private readonly string _pattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public WildcardMatcher([NotNull] string pattern, bool ignoreCase = false) : base("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", ignoreCase)
        {
            _pattern = pattern;
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <returns>Pattern</returns>
        public override string GetPattern()
        {
            return _pattern;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>
        /// Name
        /// </returns>
        public override string GetName()
        {
            return "WildcardMatcher";
        }
    }
}