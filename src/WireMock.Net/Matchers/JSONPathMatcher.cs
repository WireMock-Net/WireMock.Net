using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// JsonPathMatcher
    /// </summary>
    /// <seealso cref="IMatcher" />
    /// <seealso cref="IObjectMatcher" />
    public class JsonPathMatcher : IStringMatcher, IObjectMatcher
    {
        private readonly string[] _patterns;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public JsonPathMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="throwException">Throw an exception in case the internal matching fails.</param>
        /// <param name="patterns">The patterns.</param>
        public JsonPathMatcher(MatchBehaviour matchBehaviour, bool throwException = false, [NotNull] params string[] patterns)
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
                    var jtoken = JToken.Parse(input);
                    match = IsMatch(jtoken);
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
                try
                {
                    // Check if JToken or object
                    JToken jtoken = input is JToken token ? token : JObject.FromObject(input);
                    match = IsMatch(jtoken);
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

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public string[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "JsonPathMatcher";

        private double IsMatch(JToken jtoken)
        {
            return MatchScores.ToScore(_patterns.Select(pattern => jtoken.SelectToken(pattern) != null));
        }
    }
}