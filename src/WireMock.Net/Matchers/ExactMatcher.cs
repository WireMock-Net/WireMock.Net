using System.Linq;
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
        private readonly string[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public ExactMatcher([NotNull] params string[] values)
        {
            Check.NotNull(values, nameof(values));

            _values = values;
        }

        /// <summary>
        /// Determines whether the specified input is match.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
        public double IsMatch(string input)
        {
            return MatchScores.ToScore(_values.Select(value => value.Equals(input)));
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>Patterns</returns>
        public string[] GetPatterns()
        {
            return _values;
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