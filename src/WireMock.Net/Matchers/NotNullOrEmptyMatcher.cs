using System;
using System.Linq;
using AnyOfTypes;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// NotNullOrEmptyMatcher
/// </summary>
/// <seealso cref="IObjectMatcher" />
public class NotNullOrEmptyMatcher : IObjectMatcher, IStringMatcher
{
    /// <inheritdoc cref="IMatcher.Name"/>
    public string Name => "NotNullOrEmptyMatcher";

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotNullOrEmptyMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    public NotNullOrEmptyMatcher(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        MatchBehaviour = matchBehaviour;
    }

    /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
    public double IsMatch(object? input)
    {
        bool match;

        switch (input)
        {
            case string @string:
                match = !string.IsNullOrEmpty(@string);
                break;

            case byte[] bytes:
                match = bytes.Any();
                break;

            default:
                match = input != null;
                break;
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match));
    }

    /// <inheritdoc cref="IStringMatcher.IsMatch"/>
    public double IsMatch(string? input)
    {
        var match = !string.IsNullOrEmpty(input);

        return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match));
    }

    /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return EmptyArray<AnyOf<string, StringPattern>>.Value;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;
}