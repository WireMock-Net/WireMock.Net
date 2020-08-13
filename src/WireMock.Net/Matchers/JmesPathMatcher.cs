using DevLab.JmesPath;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Linq;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// http://jmespath.org/
    /// </summary>
    public class JmesPathMatcher : IStringMatcher, IObjectMatcher
    {
        private readonly string[] _patterns;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public JmesPathMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
        /// </summary>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        /// <param name="patterns">The patterns.</param>
        public JmesPathMatcher(bool throwException = false, [NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, throwException, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        /// <param name="patterns">The patterns.</param>
        public JmesPathMatcher(MatchBehaviour matchBehaviour, bool throwException = false, [NotNull] params string[] patterns)
        {
            Check.NotNull(patterns, nameof(patterns));

            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;
            _patterns = patterns;
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            double match = MatchScores.Mismatch;
            if (input != null)
            {
                try
                {
                    match = MatchScores.ToScore(_patterns.Select(pattern => bool.Parse(new JmesPath().Transform(input, pattern))));
                }
                catch (JsonException)
                {
                    if (ThrowException)
                    {
                        throw;
                    }
                }
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            double match = MatchScores.Mismatch;

            // When input is null or byte[], return Mismatch.
            if (input != null && !(input is byte[]))
            {
                string inputAsString = JsonConvert.SerializeObject(input);
                return IsMatch(inputAsString);
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public string[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "JmesPathMatcher";
    }
}