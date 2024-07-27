namespace WireMock.Matchers.Request;

/// <summary>
/// The request body GraphQL matcher.
/// </summary>
public interface IRequestMessageGraphQLMatcher : IRequestMatcher
{
    /// <summary>
    /// The matchers.
    /// </summary>
    IMatcher[]? Matchers { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    MatchOperator MatchOperator { get; }
}