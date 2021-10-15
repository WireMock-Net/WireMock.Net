using System.Net.Http.Headers;
using AnyOfTypes;
using JetBrains.Annotations;
using WireMock.Models;

namespace WireMock.Matchers
{
    /// <summary>
    /// ContentTypeMatcher which accepts also all charsets
    /// </summary>
    /// <seealso cref="RegexMatcher" />
    public class ContentTypeMatcher : WildcardMatcher
    {
        private readonly AnyOf<string, StringPattern>[] _patterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase (default false)</param>
        public ContentTypeMatcher([NotNull] AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(new[] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">IgnoreCase (default false)</param>
        public ContentTypeMatcher(MatchBehaviour matchBehaviour, [NotNull] AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(matchBehaviour, new[] { pattern }, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">IgnoreCase (default false)</param>
        public ContentTypeMatcher([NotNull] AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, patterns, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="ignoreCase">IgnoreCase (default false)</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        public ContentTypeMatcher(MatchBehaviour matchBehaviour, [NotNull] AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false, bool throwException = false) :
            base(matchBehaviour, patterns, ignoreCase, throwException)
        {
            _patterns = patterns;
        }

        /// <inheritdoc cref="RegexMatcher.IsMatch"/>
        public override double IsMatch(string input)
        {
            if (string.IsNullOrEmpty(input) || !MediaTypeHeaderValue.TryParse(input, out MediaTypeHeaderValue contentType))
            {
                return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.Mismatch);
            }

            return base.IsMatch(contentType.MediaType);
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public override AnyOf<string, StringPattern>[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public override string Name => "ContentTypeMatcher";
    }
}