// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => new (null, protoDefinition), messageType));
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => new(null, protoDefinition), messageType, matcher));
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => Mapping.ProtoDefinition!.Value, messageType));
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => Mapping.ProtoDefinition!.Value, messageType, matcher));
    }
}