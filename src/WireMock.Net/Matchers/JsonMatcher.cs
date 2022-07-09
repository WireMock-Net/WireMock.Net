using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// JsonMatcher
/// </summary>
public class JsonMatcher : IValueMatcher, IIgnoreCaseMatcher
{
    /// <inheritdoc cref="IValueMatcher.Value"/>
    public object Value { get; }

    /// <inheritdoc cref="IMatcher.Name"/>
    public virtual string Name => "JsonMatcher";

    /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc cref="IIgnoreCaseMatcher.IgnoreCase"/>
    public bool IgnoreCase { get; }

    /// <inheritdoc cref="IMatcher.ThrowException"/>
    public bool ThrowException { get; }

    private readonly JToken _valueAsJToken;
    private readonly Func<JToken, JToken> _jTokenConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
    /// </summary>
    /// <param name="value">The string value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    public JsonMatcher(string value, bool ignoreCase = false, bool throwException = false) : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase, throwException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
    /// </summary>
    /// <param name="value">The object value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    public JsonMatcher(object value, bool ignoreCase = false, bool throwException = false) : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase, throwException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="value">The value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    public JsonMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool throwException = false)
    {
        Guard.NotNull(value, nameof(value));

        MatchBehaviour = matchBehaviour;
        IgnoreCase = ignoreCase;
        ThrowException = throwException;

        Value = value;
        _valueAsJToken = ConvertValueToJToken(value);
        _jTokenConverter = ignoreCase ? Rename : jToken => jToken;
    }

    /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
    public double IsMatch(object? input)
    {
        bool match = false;

        // When input is null or byte[], return Mismatch.
        if (input != null && input is not byte[])
        {
            try
            {
                var inputAsJToken = ConvertValueToJToken(input);

                match = IsMatch(
                    _jTokenConverter(_valueAsJToken),
                    _jTokenConverter(inputAsJToken));
            }
            catch (JsonException)
            {
                if (ThrowException)
                {
                    throw;
                }
            }
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match));
    }

    /// <summary>
    /// Compares the input against the matcher value
    /// </summary>
    /// <param name="value">Matcher value</param>
    /// <param name="input">Input value</param>
    /// <returns></returns>
    protected virtual bool IsMatch(JToken value, JToken input)
    {
        return JToken.DeepEquals(value, input);
    }

    private static JToken ConvertValueToJToken(object value)
    {
        // Check if JToken, string, IEnumerable or object
        switch (value)
        {
            case JToken tokenValue:
                return tokenValue;

            case string stringValue:
                return JsonUtils.Parse(stringValue)!;

            case IEnumerable enumerableValue:
                return JArray.FromObject(enumerableValue);

            default:
                return JObject.FromObject(value);
        }
    }

    private static string? ToUpper(string? input)
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
                string stringValue = propertyValue.Value<string>()!;
                propertyValue = ToUpper(stringValue);
            }

            return new JProperty(ToUpper(property.Name)!, Rename(propertyValue));
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