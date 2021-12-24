using System.Linq;
using AnyOfTypes;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Extensions;
using WireMock.Models;
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
        private readonly AnyOf<string, StringPattern>[] _patterns;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public JsonPathMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, patterns.ToAnyOfPatterns())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public JsonPathMatcher([NotNull] params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        /// <param name="patterns">The patterns.</param>
        public JsonPathMatcher(MatchBehaviour matchBehaviour, bool throwException = false, [NotNull] params AnyOf<string, StringPattern>[] patterns)
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
                    var jToken = JToken.Parse(input);
                    match = IsMatch(jToken);
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
                    JToken jToken = input is JToken token ? token : JObject.FromObject(input);
                    match = IsMatch(jToken);
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
        public AnyOf<string, StringPattern>[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "JsonPathMatcher";

        private double IsMatch(JToken jToken)
        {
            return MatchScores.ToScore(_patterns.Select(pattern => jToken.SelectToken(pattern.GetPattern()) != null));
        }
    }
}