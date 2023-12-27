using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageProtoBufMatcher(matchBehaviour, protoDefinition, messageType));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, IJsonMatcher jsonMatcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageProtoBufMatcher(matchBehaviour, protoDefinition, messageType, jsonMatcher));
        return this;
    }
}