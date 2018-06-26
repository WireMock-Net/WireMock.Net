using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// JsonMatcher
    /// </summary>
    public class JsonObjectMatcher : IValueMatcher
    {
        private readonly object _value;

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "JsonObjectMatcher";

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="value">The value to check for equality.</param>
        public JsonObjectMatcher([NotNull] object value) : this(MatchBehaviour.AcceptOnMatch, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The value to check for equality.</param>
        public JsonObjectMatcher(MatchBehaviour matchBehaviour, [NotNull] object value)
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
                    match = AreDeepEquals(input, _value);
                }
                catch (JsonException)
                {
                    // just ignore JsonException
                }
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match));
        }

        private bool AreDeepEquals(object specimen, object target)
        {
            var settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };

            return JsonConvert.SerializeObject(specimen, settings) == JsonConvert.SerializeObject(target, settings);
        }

        /// <inheritdoc cref="IValueMatcher.GetValue"/>
        public object GetValue() => _value;
    }
}