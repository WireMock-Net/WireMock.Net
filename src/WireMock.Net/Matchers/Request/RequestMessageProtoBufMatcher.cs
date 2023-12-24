namespace WireMock.Matchers.Request;

/// <summary>
/// The request body Grpc ProtoBuf matcher.
/// </summary>
public class RequestMessageProtoBufMatcher : IRequestMatcher
{
    /// <summary>
    /// The ProtoBufMatcher.
    /// </summary>
    public IBytesMatcher? Matcher { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageProtoBufMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <param name="protoDefinition">The proto definition as a string.</param>
    /// <param name="grpcServiceMethod">The method which is called on service. Format is "{package-name}.{service-name}-{method-name}".</param>
    /// <param name="jsonMatcher">The optional jsonMatcher to use to match the ProtoBuf as (json) object.</param>
    public RequestMessageProtoBufMatcher(MatchBehaviour matchBehaviour, string protoDefinition, string grpcServiceMethod, IJsonMatcher? jsonMatcher = null)
    {
#if PROTOBUF
        Matcher = new ProtoBufMatcher(protoDefinition, grpcServiceMethod, matchBehaviour, jsonMatcher);
#else
        throw new System.NotSupportedException("The ProtoBufMatcher can not be used for .NETStandard1.3 or .NET Framework 4.6.1 or lower.");
#endif
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = GetMatchResult(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private MatchResult GetMatchResult(IRequestMessage requestMessage)
    {
        return Matcher?.IsMatch(requestMessage.BodyAsBytes) ?? default;
    }
}