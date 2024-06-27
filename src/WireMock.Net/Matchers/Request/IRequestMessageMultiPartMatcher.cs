namespace WireMock.Matchers.Request;

/// <summary>
/// The request body MultiPart matcher.
/// </summary>
public interface IRequestMessageMultiPartMatcher : IRequestMatcher
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