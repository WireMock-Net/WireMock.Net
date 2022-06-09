using System;
using System.Linq;
using Stef.Validation;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request method matcher.
/// </summary>
internal class RequestMessageMethodMatcher : IRequestMatcher
{
    /// <summary>
    /// The <see cref="Matchers.MatchBehaviour"/>
    /// </summary>
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// The <see cref="Matchers.MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; }

    /// <summary>
    /// The methods
    /// </summary>
    public string[] Methods { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageMethodMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use.</param>
    /// <param name="methods">The methods.</param>
    public RequestMessageMethodMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params string[] methods)
    {
        Methods = Guard.NotNull(methods);
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        double score = MatchBehaviourHelper.Convert(MatchBehaviour, IsMatch(requestMessage));
        return requestMatchResult.AddScore(GetType(), score);
    }

    private double IsMatch(IRequestMessage requestMessage)
    {
        var scores = Methods.Select(m => string.Equals(m, requestMessage.Method, StringComparison.OrdinalIgnoreCase)).ToArray();
        return MatchScores.ToScore(scores, MatchOperator);
    }
}