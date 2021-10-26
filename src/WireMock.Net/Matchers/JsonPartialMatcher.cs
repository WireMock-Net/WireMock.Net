using JetBrains.Annotations;

namespace WireMock.Matchers
{
    /// <summary>
    /// JsonPartialMatcher
    /// </summary>
    public class JsonPartialMatcher : AbstractJsonPartialMatcher
    {
        /// <inheritdoc />
        public override string Name => nameof(JsonPartialMatcher);

        /// <inheritdoc />
        public JsonPartialMatcher([NotNull] string value, bool ignoreCase = false, bool throwException = false)
            : base(value, ignoreCase, throwException)
        {
        }

        /// <inheritdoc />
        public JsonPartialMatcher([NotNull] object value, bool ignoreCase = false, bool throwException = false)
            : base(value, ignoreCase, throwException)
        {
        }

        /// <inheritdoc />
        public JsonPartialMatcher(MatchBehaviour matchBehaviour, [NotNull] object value, bool ignoreCase = false, bool throwException = false)
            : base(matchBehaviour, value, ignoreCase, throwException)
        {
        }

        /// <inheritdoc />
        protected override bool IsMatch(string value, string input)
        {
            var exactStringMatcher = new ExactMatcher(MatchBehaviour.AcceptOnMatch, ThrowException, value);
            return MatchScores.IsPerfect(exactStringMatcher.IsMatch(input));
        }
    }
}
