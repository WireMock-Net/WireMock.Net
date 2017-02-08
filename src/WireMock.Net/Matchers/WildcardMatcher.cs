using System.Linq;
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
        private readonly string[] _patterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public WildcardMatcher([NotNull] string pattern, bool ignoreCase = false) : this(new [] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public WildcardMatcher([NotNull] string[] patterns, bool ignoreCase = false) : base(patterns.Select(pattern => "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$").ToArray(), ignoreCase)
        {
            _patterns = patterns;
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <returns>Pattern</returns>
        public override string[] GetPatterns()
        {
            return _patterns;
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