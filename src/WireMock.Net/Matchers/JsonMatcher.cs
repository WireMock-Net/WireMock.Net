using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        public JsonMatcher([NotNull] string value, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="value">The object value to check for equality.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
        public JsonMatcher([NotNull] object value, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="value">The string value to check for equality.</param>
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
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
        /// <param name="ignoreCase">Ignore the case from the pattern.</param>
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
                            jtokenValue = JToken.Parse(stringValue);
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

        private bool DeepEquals(JToken jtokenValue, JToken jtokenInput)
        {
            if (!IgnoreCase)
            {
                return JToken.DeepEquals(jtokenValue, jtokenInput);
            }

            JToken jtokenValueCloned = jtokenValue.DeepClone();
            JToken jtokenInputCloned = jtokenInput.DeepClone();

            WalkNodeAndUpdateStringValuesToUpper(jtokenValueCloned);
            WalkNodeAndUpdateStringValuesToUpper(jtokenInputCloned);

            return JToken.DeepEquals(jtokenValueCloned, jtokenInputCloned);
        }

        private static void WalkNodeAndUpdateStringValuesToUpper(JToken node)
        {
            if (node.Type == JTokenType.Object)
            {
                // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JProperty child in node.Children<JProperty>().ToArray())
                {
                    WalkNodeAndUpdateStringValuesToUpper(child.Value);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JToken child in node.Children().ToArray())
                {
                    WalkNodeAndUpdateStringValuesToUpper(child);
                }
            }
            else if (node.Type == JTokenType.String)
            {
                // In case of string, do a ToUpperInvariant().
                string stringValue = node.Value<string>();
                if (!string.IsNullOrEmpty(stringValue))
                {
                    string stringValueAsUpper = stringValue.ToUpperInvariant();
                    JToken value;
                    try
                    {
                        // Try to convert this string into a JsonObject
                        value = JToken.Parse(stringValueAsUpper);
                    }
                    catch (JsonException)
                    {
                        // Ignore JsonException and just keep string value and convert to JToken
                        value = stringValueAsUpper;
                    }

                    node.Replace(value);
                }
            }
        }
    }
}