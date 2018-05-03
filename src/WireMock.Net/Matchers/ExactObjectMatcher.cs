using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

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

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher([NotNull] object value) : this(MatchBehaviour.AcceptOnMatch, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher(MatchBehaviour matchBehaviour, [NotNull] object value)
        {
            Check.NotNull(value, nameof(value));

            _object = value;
            MatchBehaviour = matchBehaviour;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher([NotNull] byte[] value) : this(MatchBehaviour.AcceptOnMatch, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher(MatchBehaviour matchBehaviour, [NotNull] byte[] value)
        {
            Check.NotNull(value, nameof(value));

            _bytes = value;
            MatchBehaviour = matchBehaviour;
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            bool equals = _object != null ? Equals(_object, input) : _bytes.SequenceEqual((byte[])input);
            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(equals));
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "ExactObjectMatcher";
    }
}