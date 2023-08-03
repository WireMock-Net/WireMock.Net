using System.Linq;
using AnyOfTypes;
using DevLab.JmesPath;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// http://jmespath.org/
/// </summary>
public class JmesPathMatcher : IStringMatcher, IObjectMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public JmesPathMatcher(params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, patterns.ToAnyOfPatterns())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public JmesPathMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
    /// </summary>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use.</param>
    /// <param name="patterns">The patterns.</param>
    public JmesPathMatcher(bool throwException = false, MatchOperator matchOperator = MatchOperator.Or, params AnyOf<string, StringPattern>[] patterns) :
        this(MatchBehaviour.AcceptOnMatch, throwException, matchOperator, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use.</param>
    /// <param name="patterns">The patterns.</param>
    public JmesPathMatcher(
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
                var results = _patterns.Select(pattern => bool.Parse(new JmesPath().Transform(input, pattern.GetPattern()))).ToArray();
                match = MatchScores.ToScore(results, MatchOperator);
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
            string inputAsString = JsonConvert.SerializeObject(input);
            return IsMatch(inputAsString);
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, match);
    }

    /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc cref="IMatcher.Name"/>
    public string Name => "JmesPathMatcher";
}