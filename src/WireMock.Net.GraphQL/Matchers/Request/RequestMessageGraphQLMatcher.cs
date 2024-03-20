#if GRAPHQL
using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body GraphQL matcher.
/// </summary>
public class RequestMessageGraphQLMatcher : IRequestMessageGraphQLMatcher
{
    /// <inheritdoc />
    public IMatcher[]? Matchers { get; }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. [optional]</param>
    public RequestMessageGraphQLMatcher(MatchBehaviour matchBehaviour, string schema, IDictionary<string, Type>? customScalars = null) :
        this(CreateMatcherArray(matchBehaviour, schema, customScalars))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. [optional]</param>
    public RequestMessageGraphQLMatcher(MatchBehaviour matchBehaviour, GraphQL.Types.ISchema schema, IDictionary<string, Type>? customScalars = null) :
        this(CreateMatcherArray(matchBehaviour, new AnyOfTypes.AnyOf<string, WireMock.Models.StringPattern, object?>(schema), customScalars))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageGraphQLMatcher(params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    public RequestMessageGraphQLMatcher(MatchOperator matchOperator, params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var results = CalculateMatchResults(requestMessage);
        var (score, exception) = MatchResult.From(results, MatchOperator).Expand();

        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private static MatchResult CalculateMatchResult(IRequestMessage requestMessage, IMatcher matcher)
    {
        // Check if the matcher is a IStringMatcher
        // If the body is a Json or a String, use the BodyAsString to match on.
        if (matcher is IStringMatcher stringMatcher && requestMessage.BodyData?.DetectedBodyType is BodyType.Json or BodyType.String or BodyType.FormUrlEncoded)
        {
            return stringMatcher.IsMatch(requestMessage.BodyData.BodyAsString);
        }

        return default;
    }

    private IReadOnlyList<MatchResult> CalculateMatchResults(IRequestMessage requestMessage)
    {
        return Matchers == null ? new[] { new MatchResult() } : Matchers.Select(matcher => CalculateMatchResult(requestMessage, matcher)).ToArray();
    }

    private static IMatcher[] CreateMatcherArray(MatchBehaviour matchBehaviour, AnyOfTypes.AnyOf<string, WireMock.Models.StringPattern, object?> schema, IDictionary<string, Type>? customScalars)
    {
        return new[] { TypeLoader.Load<IGraphQLMatcher>(schema, customScalars, matchBehaviour, MatchOperator.Or) }.Cast<IMatcher>().ToArray();
    }
}
#endif