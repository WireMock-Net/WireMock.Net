using System.Linq;
using AnyOfTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// JsonPathMatcher
/// </summary>
/// <seealso cref="IMatcher" />
/// <seealso cref="IObjectMatcher" />
public class JsonPathMatcher : IStringMatcher, IObjectMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public JsonPathMatcher(params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, patterns.ToAnyOfPatterns())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public JsonPathMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="patterns">The patterns.</param>
    public JsonPathMatcher(
        MatchBehaviour matchBehaviour,
        bool throwException = false,
        MatchOperator matchOperator = MatchOperator.Or,
        params AnyOf<string, StringPattern>[] patterns)
    {
        _patterns = Guard.NotNull(patterns);
        MatchBehaviour = matchBehaviour;
        ThrowException = throwException;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc cref="IStringMatcher.IsMatch"/>
    public double IsMatch(string? input)
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
    public double IsMatch(object? input)
    {
        double match = MatchScores.Mismatch;

        // When input is null or byte[], return Mismatch.
        if (input != null && !(input is byte[]))
        {
            try
            {
                // Check if JToken or object
                JToken jToken = input as JToken ?? JObject.FromObject(input);
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

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc cref="IMatcher.Name"/>
    public string Name => "JsonPathMatcher";

    private double IsMatch(JToken jToken)
    {
        var array = ConvertJTokenToJArrayIfNeeded(jToken);

        return MatchScores.ToScore(_patterns.Select(pattern => array.SelectToken(pattern.GetPattern())?.Any() == true).ToArray(), MatchOperator);
    }

    // https://github.com/WireMock-Net/WireMock.Net/issues/965
    // https://stackoverflow.com/questions/66922188/newtonsoft-jsonpath-with-c-sharp-syntax
    // Filtering using SelectToken() isn't guaranteed to work for objects inside objects -- only objects inside arrays.
    // So this code checks if it's an JArray, if it's not an array, construct a new JArray.
    private static JToken ConvertJTokenToJArrayIfNeeded(JToken jToken)
    {
        if (jToken.Count() == 1)
        {
            var property = jToken.First();
            var item = property.First();
            if (item is JArray)
            {
               return jToken;
            }
            
            return new JObject
            {
                [property.Path] = new JArray(item)
            };
        }

        return jToken;
    }
}