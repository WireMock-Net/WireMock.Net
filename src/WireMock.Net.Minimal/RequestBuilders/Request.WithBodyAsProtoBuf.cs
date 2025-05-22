// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsProtoBuf([protoDefinition], messageType, matchBehaviour);
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsProtoBuf([protoDefinition], messageType, matcher, matchBehaviour);
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(IReadOnlyList<string> protoDefinitions, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => new IdOrTexts(null, protoDefinitions), messageType));
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(IReadOnlyList<string> protoDefinitions, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => new IdOrTexts(null, protoDefinitions), messageType, matcher));
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageProtoBufMatcher(matchBehaviour, ProtoDefinitionFunc(), messageType));
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsProtoBuf(string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageProtoBufMatcher(matchBehaviour, ProtoDefinitionFunc(), messageType, matcher));
    }

    private Func<IdOrTexts> ProtoDefinitionFunc()
    {
        return () =>
        {
            if (Mapping.ProtoDefinition == null)
            {
                throw new InvalidOperationException($"No ProtoDefinition defined on mapping '{Mapping.Guid}'. Please use the WireMockServerSettings to define ProtoDefinitions.");
            }

            return Mapping.ProtoDefinition.Value;
        };
    }
}