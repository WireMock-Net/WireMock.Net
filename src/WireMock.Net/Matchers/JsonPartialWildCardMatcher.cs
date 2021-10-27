using JetBrains.Annotations;

namespace WireMock.Matchers
{
    /// <summary>
    /// JsonPartialWildCardMatcher
    /// </summary>
    public class JsonPartialWildcardMatcher : AbstractJsonPartialMatcher
    {
        /// <inheritdoc />
        public override string Name => nameof(JsonPartialWildcardMatcher);

        /// <inheritdoc />
        public JsonPartialWildcardMatcher([NotNull] string value, bool ignoreCase = false, bool throwException = false)
            : base(value, ignoreCase, throwException)
        {
        }

        /// <inheritdoc />
        public JsonPartialWildcardMatcher([NotNull] object value, bool ignoreCase = false, bool throwException = false)
            : base(value, ignoreCase, throwException)
        {
        }

        /// <inheritdoc />
        public JsonPartialWildcardMatcher(MatchBehaviour matchBehaviour, [NotNull] object value, bool ignoreCase = false, bool throwException = false)
            : base(matchBehaviour, value, ignoreCase, throwException)
        {
        }

        /// <inheritdoc />
        protected override bool IsMatch(string value, string input)
        {
            var wildcardStringMatcher = new WildcardMatcher(MatchBehaviour.AcceptOnMatch, value, IgnoreCase);
            return MatchScores.IsPerfect(wildcardStringMatcher.IsMatch(input));
        }
    }
}
