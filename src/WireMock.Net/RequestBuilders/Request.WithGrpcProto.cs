using System;
using WireMock.Matchers;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithGrpcProto(string protoDefinition, string grpcServiceMethod, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IRequestBuilder WithGrpcProto(string protoDefinition, string grpcServiceMethod, IJsonMatcher jsonMatcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        throw new NotImplementedException();
    }
}