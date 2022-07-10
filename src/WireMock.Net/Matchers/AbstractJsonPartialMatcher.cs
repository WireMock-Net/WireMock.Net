using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// Generic AbstractJsonPartialMatcher
/// </summary>
public abstract class AbstractJsonPartialMatcher : JsonMatcher
{
    private const string Prefix = "WireMockRegex:";

    /// <summary>
    /// Support Regex
    /// </summary>
    public bool Regex { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractJsonPartialMatcher"/> class.
    /// </summary>
    /// <param name="value">The string value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    protected AbstractJsonPartialMatcher(string value, bool ignoreCase = false, bool throwException = false)
        : base(value, ignoreCase, throwException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractJsonPartialMatcher"/> class.
    /// </summary>
    /// <param name="value">The object value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    protected AbstractJsonPartialMatcher(object value, bool ignoreCase = false, bool throwException = false)
        : base(value, ignoreCase, throwException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractJsonPartialMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="value">The value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    protected AbstractJsonPartialMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool throwException = false)
        : base(matchBehaviour, value, ignoreCase, throwException)
    {
    }

    /// <inheritdoc />
    protected override bool IsMatch(JToken? value, JToken? input)
    {
        if (value == null || value == input)
        {
            return true;
        }

        if (Regex && value.Type == JTokenType.String && input != null)
        {
            var valueAsString = value.ToString();
            if (valueAsString.StartsWith(Prefix))
            {
                var (valid, result) = RegexUtils.MatchRegex(valueAsString.Substring(Prefix.Length), input.ToString());
                if (valid)
                {
                    return result;
                }
            }
        }

        if (input == null || value.Type != input.Type)
        {
            return false;
        }

        switch (value.Type)
        {
            case JTokenType.Object:
                var nestedValues = value.ToObject<Dictionary<string, JToken>>();
                return nestedValues?.Any() != true ||
                       nestedValues.All(pair => IsMatch(pair.Value, input.SelectToken(pair.Key)));

            case JTokenType.Array:
                var valuesArray = value.ToObject<JToken[]>();
                var tokenArray = input.ToObject<JToken[]>();

                if (valuesArray?.Any() != true)
                {
                    return true;
                }

                return tokenArray?.Any() == true &&
                       valuesArray.All(subFilter => tokenArray.Any(subToken => IsMatch(subFilter, subToken)));

            default:
                return IsMatch(value.ToString(), input.ToString());
        }
    }

    /// <summary>
    /// Check if two strings are a match (matching can be done exact or wildcard)
    /// </summary>
    protected abstract bool IsMatch(string value, string input);
}