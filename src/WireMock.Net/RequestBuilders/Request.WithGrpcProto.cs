using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithGrpcProto(string protoDefinition, string grpcServiceMethod, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageProtoBufMatcher(matchBehaviour, protoDefinition, grpcServiceMethod));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithGrpcProto(string protoDefinition, string grpcServiceMethod, IJsonMatcher jsonMatcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageProtoBufMatcher(matchBehaviour, protoDefinition, grpcServiceMethod, jsonMatcher));
        return this;
    }
}