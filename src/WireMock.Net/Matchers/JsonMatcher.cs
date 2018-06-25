using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// JsonMatcher
    /// </summary>
    public class JsonMatcher : IValueMatcher
    {
        private readonly string _value;

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "JsonMatcher";

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="value">The value to check for equality.</param>
        public JsonMatcher([NotNull] string value) : this(MatchBehaviour.AcceptOnMatch, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The value to check for equality.</param>
        public JsonMatcher(MatchBehaviour matchBehaviour, [NotNull] string value)
        {
            Check.NotNull(value, nameof(value));

            MatchBehaviour = matchBehaviour;
            _value = value;
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            bool match = false;
            if (input != null)
            {
                try
                {
                    // Check if JToken or object
                    JToken jtoken = input is JToken token ? token : JObject.FromObject(input);

                    match = JToken.DeepEquals(JToken.Parse(_value), jtoken);
                }
                catch (JsonException)
                {
                    // just ignore JsonException
                }
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match));
        }

        /// <inheritdoc cref="IValueMatcher.GetValue"/>
        public string GetValue() => _value;
    }
}