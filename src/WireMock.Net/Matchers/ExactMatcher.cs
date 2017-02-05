using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// ExactMatcher
    /// </summary>
    /// <seealso cref="IMatcher" />
    public class ExactMatcher : IMatcher
    {
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExactMatcher([NotNull] string value)
        {
            Check.NotNull(value, nameof(value));

            _value = value;
        }

        /// <summary>
        /// Determines whether the specified input is match.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
        public double IsMatch(string input)
        {
            return MatchScores.ToScore(_value.Equals(input));
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>Pattern</returns>
        public string GetPattern()
        {
            return _value;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name</returns>
        public string GetName()
        {
            return "ExactMatcher";
        }
    }
}