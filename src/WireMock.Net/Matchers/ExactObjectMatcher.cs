using JetBrains.Annotations;

namespace WireMock.Matchers
{
    /// <summary>
    /// ExactMatcher
    /// </summary>
    /// <seealso cref="IObjectMatcher" />
    public class ExactObjectMatcher : IObjectMatcher
    {
        private readonly object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher([NotNull] object value)
        {
            _value = value;
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            return MatchScores.ToScore(Equals(_value, input));
        }

        /// <inheritdoc cref="IMatcher.GetName"/>
        public string GetName()
        {
            return "ExactObjectMatcher";
        }
    }
}