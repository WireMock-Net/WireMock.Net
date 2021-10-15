using System.Linq;
using System.Text.RegularExpressions;
using AnyOfTypes;
using JetBrains.Annotations;
using WireMock.Extensions;
using WireMock.Models;

namespace WireMock.Matchers
{
    /// <summary>
    /// WildcardMatcher
    /// </summary>
    /// <seealso cref="RegexMatcher" />
    public class WildcardMatcher : RegexMatcher
    {
        private readonly AnyOf<string, StringPattern>[] _patterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public WildcardMatcher([NotNull] AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(new[] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public WildcardMatcher(MatchBehaviour matchBehaviour, [NotNull] AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(matchBehaviour, new[] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        public WildcardMatcher([NotNull] AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, patterns, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">IgnoreCase</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        public WildcardMatcher(MatchBehaviour matchBehaviour, [NotNull] AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false, bool throwException = false) :
            base(matchBehaviour, CreateArray(patterns), ignoreCase, throwException)
        {
            _patterns = patterns;
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public override AnyOf<string, StringPattern>[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public override string Name => "WildcardMatcher";

        private static AnyOf<string, StringPattern>[] CreateArray(AnyOf<string, StringPattern>[] patterns)
        {
            return patterns.Select(pattern => new AnyOf<string, StringPattern>(
                new StringPattern
                {
                    Pattern = "^" + Regex.Escape(pattern.GetPattern()).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                    PatternAsFile = pattern.IsSecond ? pattern.Second.PatternAsFile : null
                }))
                .ToArray();
        }
    }
}