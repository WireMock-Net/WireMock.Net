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

        /// <summary>
        /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public JmesPathMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        public JmesPathMatcher(MatchBehaviour matchBehaviour, [NotNull] params string[] patterns)
        {
            Check.NotNull(patterns, nameof(patterns));

            MatchBehaviour = matchBehaviour;
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
                    // just ignore JsonException
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