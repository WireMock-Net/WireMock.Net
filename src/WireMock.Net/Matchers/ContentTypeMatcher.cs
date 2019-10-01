using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace WireMock.Matchers
{
    /// <summary>
    /// ContentTypeMatcher which accepts also all charsets
    /// </summary>
    /// <seealso cref="RegexMatcher" />
    public class ContentTypeMatcher : RegexMatcher
    {
        private readonly string[] _patterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase (default false)</param>
        public ContentTypeMatcher([NotNull] string pattern, bool ignoreCase = false) : this(new[] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase (default false)</param>
        public ContentTypeMatcher(MatchBehaviour matchBehaviour, [NotNull] string pattern, bool ignoreCase = false) : this(matchBehaviour, new[] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">IgnoreCase (default false)</param>
        public ContentTypeMatcher([NotNull] string[] patterns, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, patterns, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">IgnoreCase (default false)</param>
        public ContentTypeMatcher(MatchBehaviour matchBehaviour, [NotNull] string[] patterns, bool ignoreCase = false) :
            base(matchBehaviour, patterns.Select(pattern => $"^{Regex.Escape(pattern)}(\\s*;\\s*charset=[a-zA-Z0-9-]+\\s*)?$").ToArray(), ignoreCase)
        {
            _patterns = patterns;
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public override string[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public override string Name => "ContentTypeMatcher";
    }
}