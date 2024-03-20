namespace WireMock.Matchers.Request;

/// <summary>
/// The request body ProtoBuf matcher.
/// </summary>
public interface IRequestMessageProtoBufMatcher : IRequestMatcher
{
    /// <summary>
    /// The ProtoBufMatcher.
    /// </summary>
    IProtoBufMatcher? Matcher { get; }
}