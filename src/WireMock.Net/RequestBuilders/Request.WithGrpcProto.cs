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
    public IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageProtoBufMatcher(matchBehaviour, protoDefinition, messageType, matcher));
        return this;
    }
}