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

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public ExactMatcher([NotNull] params string[] values) : this(MatchBehaviour.AcceptOnMatch, values)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="values">The values.</param>
        public ExactMatcher(MatchBehaviour matchBehaviour, [NotNull] params string[] values)
        {
            Check.HasNoNulls(values, nameof(values));

            _values = values;
            MatchBehaviour = matchBehaviour;
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(_values.Select(value => value.Equals(input))));
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