using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// JsonMatcher
    /// </summary>
    public class JsonMatcher : IValueMatcher, IIgnoreCaseMatcher
    {
        /// <inheritdoc cref="IValueMatcher.Value"/>
        public object Value { get; }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "JsonMatcher";

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IIgnoreCaseMatcher.IgnoreCase"/>
        public bool IgnoreCase { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="value">The string value to check for equality.</param>
        /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
        public JsonMatcher([NotNull] string value, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="value">The object value to check for equality.</param>
        /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
        public JsonMatcher([NotNull] object value, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The string value to check for equality.</param>
        /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
        public JsonMatcher(MatchBehaviour matchBehaviour, [NotNull] string value, bool ignoreCase = false)
        {
            Check.NotNull(value, nameof(value));

            MatchBehaviour = matchBehaviour;
            Value = value;
            IgnoreCase = ignoreCase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The object value to check for equality.</param>
        /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
        public JsonMatcher(MatchBehaviour matchBehaviour, [NotNull] object value, bool ignoreCase = false)
        {
            Check.NotNull(value, nameof(value));

            MatchBehaviour = matchBehaviour;
            Value = value;
            IgnoreCase = ignoreCase;
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            bool match = false;

            // When input is null or byte[], return Mismatch.
            if (input != null && !(input is byte[]))
            {
                try
                {
                    // Check if JToken or object
                    JToken jtokenInput = input is JToken tokenInput ? tokenInput : JObject.FromObject(input);

                    // Check if JToken or string or object
                    JToken jtokenValue;
                    switch (Value)
                    {
                        case JToken tokenValue:
                            jtokenValue = tokenValue;
                            break;

                        case string stringValue:
                            jtokenValue = JsonUtils.Parse(stringValue);
                            break;

                        default:
                            jtokenValue = JObject.FromObject(Value);
                            break;
                    }

                    match = DeepEquals(jtokenValue, jtokenInput);
                }
                catch (JsonException)
                {
                    // just ignore JsonException
                }
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match));
        }

        private bool DeepEquals(JToken value, JToken input)
        {
            if (!IgnoreCase)
            {
                return JToken.DeepEquals(value, input);
            }

            JToken renamedValue = Rename(value);
            JToken renamedInput = Rename(input);

            return JToken.DeepEquals(renamedValue, renamedInput);
        }

        private static string ToUpper(string input)
        {
            return input?.ToUpperInvariant();
        }

        // https://stackoverflow.com/questions/11679804/json-net-rename-properties
        private static JToken Rename(JToken json)
        {
            if (json is JProperty property)
            {
                JToken propertyValue = property.Value;
                if (propertyValue.Type == JTokenType.String)
                {
                    string stringValue = propertyValue.Value<string>();
                    propertyValue = ToUpper(stringValue);
                }

                return new JProperty(ToUpper(property.Name), Rename(propertyValue));
            }

            if (json is JArray array)
            {
                var renamedValues = array.Select(Rename);
                return new JArray(renamedValues);
            }

            if (json is JObject obj)
            {
                var renamedProperties = obj.Properties().Select(Rename);
                return new JObject(renamedProperties);
            }

            return json;
        }
    }
}