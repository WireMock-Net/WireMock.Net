using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// ExactMatcher
    /// </summary>
    /// <seealso cref="IStringMatcher" />
    public class ExactMatcher : IStringMatcher
    {
        private readonly string[] _values;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public ExactMatcher([NotNull] params string[] values) : this(MatchBehaviour.AcceptOnMatch, false, values)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="throwException">Throw an exception in case the internal matching fails.</param>
        /// <param name="values">The values.</param>
        public ExactMatcher(MatchBehaviour matchBehaviour, bool throwException = false, [NotNull] params string[] values)
        {
            Check.HasNoNulls(values, nameof(values));

            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;
            _values = values;
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            if (_values.Length == 1)
            {
                return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(_values[0] == input));
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(_values.Contains(input)));
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public string[] GetPatterns()
        {
            return _values;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "ExactMatcher";
    }
}