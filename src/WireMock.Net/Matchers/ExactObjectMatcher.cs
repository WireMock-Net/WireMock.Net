using System.Linq;
using JetBrains.Annotations;

namespace WireMock.Matchers
{
    /// <summary>
    /// ExactMatcher
    /// </summary>
    /// <seealso cref="IObjectMatcher" />
    public class ExactObjectMatcher : IObjectMatcher
    {
        private readonly object _object;
        private readonly byte[] _bytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher([NotNull] object value)
        {
            _object = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher([NotNull] byte[] value)
        {
            _bytes = value;
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            bool equals = _object != null ? Equals(_object, input) : _bytes.SequenceEqual((byte[])input);
            return MatchScores.ToScore(equals);
        }

        /// <inheritdoc cref="IMatcher.GetName"/>
        public string GetName()
        {
            return "ExactObjectMatcher";
        }
    }
}