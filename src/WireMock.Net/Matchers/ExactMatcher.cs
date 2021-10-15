using System.Linq;
using AnyOfTypes;
using JetBrains.Annotations;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// ExactMatcher
    /// </summary>
    /// <seealso cref="IStringMatcher" />
    public class ExactMatcher : IStringMatcher
    {
        private readonly AnyOf<string, StringPattern>[] _values;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public ExactMatcher([NotNull] params AnyOf<string, StringPattern>[] values) : this(MatchBehaviour.AcceptOnMatch, false, values)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        /// <param name="values">The values.</param>
        public ExactMatcher(MatchBehaviour matchBehaviour, bool throwException = false, [NotNull] params AnyOf<string, StringPattern>[] values)
        {
            Check.NotNull(values, nameof(values));

            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;
            _values = values;
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            if (_values.Length == 1)
            {
                return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(_values[0].GetPattern() == input));
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(_values.Select(v => v.GetPattern()).Contains(input)));
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public AnyOf<string, StringPattern>[] GetPatterns()
        {
            return _values;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "ExactMatcher";
    }
}