namespace WireMock.Matchers.Request;

/// <summary>
/// The request body Grpc ProtoBuf matcher.
/// </summary>
public class RequestMessageProtoBufMatcher : IRequestMatcher
{
    /// <summary>
    /// The matcher.
    /// </summary>
    public IMatcher Matcher { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageProtoBufMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <param name="protoDefinition">The proto definition as a string.</param>
    /// <param name="grpcServiceMethod">The method which is called on service. Format is "{package-name}.{service-name}-{method-name}".</param>
    /// <param name="jsonMatcher">The optional jsonMatcher to use to match the ProtoBuf as (json) object.</param>
    public RequestMessageProtoBufMatcher(MatchBehaviour matchBehaviour, string protoDefinition, string grpcServiceMethod, IJsonMatcher? jsonMatcher = null)
    {
        Matcher = CreateMatcher(matchBehaviour, protoDefinition, grpcServiceMethod, jsonMatcher);
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        throw new System.NotImplementedException();
    }


    private static IMatcher CreateMatcher(
        MatchBehaviour matchBehaviour,
        string protoDefinition,
        string grpcServiceMethod,
        IJsonMatcher? jsonMatcher
    )
    {
#if PROTOBUF
        return new ProtoBufMatcher(protoDefinition, grpcServiceMethod, matchBehaviour, jsonMatcher);
#else
        throw new System.NotSupportedException("The ProtoBufMatcher can not be used for .NETStandard1.3 or .NET Framework 4.6.1 or lower.");
#endif
    }
}