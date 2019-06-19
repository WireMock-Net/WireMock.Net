using JetBrains.Annotations;
using System.Linq;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// ExactObjectMatcher
    /// </summary>
    /// <seealso cref="IObjectMatcher" />
    public class ExactObjectMatcher : IObjectMatcher
    {
        /// <summary>
        /// Gets the value as object.
        /// </summary>
        public object ValueAsObject { get; }

        /// <summary>
        /// Gets the value as byte[].
        /// </summary>
        public byte[] ValueAsBytes { get; }

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher([NotNull] object value) : this(MatchBehaviour.AcceptOnMatch, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher(MatchBehaviour matchBehaviour, [NotNull] object value)
        {
            Check.NotNull(value, nameof(value));

            ValueAsObject = value;
            MatchBehaviour = matchBehaviour;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher([NotNull] byte[] value) : this(MatchBehaviour.AcceptOnMatch, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The value.</param>
        public ExactObjectMatcher(MatchBehaviour matchBehaviour, [NotNull] byte[] value)
        {
            Check.NotNull(value, nameof(value));

            ValueAsBytes = value;
            MatchBehaviour = matchBehaviour;
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            bool equals = ValueAsObject != null ? Equals(ValueAsObject, input) : ValueAsBytes.SequenceEqual((byte[])input);
            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(equals));
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "ExactObjectMatcher";
    }
}