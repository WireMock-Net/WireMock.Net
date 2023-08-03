using System.Linq;
using Stef.Validation;
using WireMock.Types;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body GraphQL matcher.
/// </summary>
public class RequestMessageGraphQLMatcher : IRequestMatcher
{
    /// <summary>
    /// The matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="schema">The schema.</param>
    public RequestMessageGraphQLMatcher(MatchBehaviour matchBehaviour, string schema) :
        this(CreateMatcherArray(matchBehaviour, schema))
    {
    }

#if GRAPHQL
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="schema">The schema.</param>
    public RequestMessageGraphQLMatcher(MatchBehaviour matchBehaviour, GraphQL.Types.ISchema schema) :
        this(CreateMatcherArray(matchBehaviour, new AnyOfTypes.AnyOf<string, Models.StringPattern, GraphQL.Types.ISchema>(schema)))
    {
    }
#endif

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
        var score = CalculateMatchScore(requestMessage);
        return requestMatchResult.AddScore(GetType(), score);
    }

    private static double CalculateMatchScore(IRequestMessage requestMessage, IMatcher matcher)
    {
        // Check if the matcher is a IStringMatcher
        // If the body is a Json or a String, use the BodyAsString to match on.
        if (matcher is IStringMatcher stringMatcher && requestMessage.BodyData?.DetectedBodyType is BodyType.Json or BodyType.String or BodyType.FormUrlEncoded)
        {
            return stringMatcher.IsMatch(requestMessage.BodyData.BodyAsString);
        }

        return MatchScores.Mismatch;
    }

    private double CalculateMatchScore(IRequestMessage requestMessage)
    {
        if (Matchers == null)
        {
            return MatchScores.Mismatch;
        }

        var matchersResult = Matchers.Select(matcher => CalculateMatchScore(requestMessage, matcher)).ToArray();
        return MatchScores.ToScore(matchersResult, MatchOperator);
    }

#if GRAPHQL
    private static IMatcher[] CreateMatcherArray(MatchBehaviour matchBehaviour, AnyOfTypes.AnyOf<string, Models.StringPattern, GraphQL.Types.ISchema> schema)
    {
        return new[] { new GraphQLMatcher(schema, matchBehaviour) }.Cast<IMatcher>().ToArray();
    }
#else
    private static IMatcher[] CreateMatcherArray(MatchBehaviour matchBehaviour, object schema)
    {
        throw new System.NotSupportedException("The GrapQLMatcher can not be used for .NETStandard1.3 or .NET Framework 4.6.1 or lower.");
    }
#endif
}