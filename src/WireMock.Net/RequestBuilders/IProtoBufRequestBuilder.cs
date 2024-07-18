// Copyright © WireMock.Net

using WireMock.Matchers;

namespace WireMock.RequestBuilders;

/// <summary>
/// The ProtoBufRequestBuilder interface.
/// </summary>
public interface IProtoBufRequestBuilder : IGraphQLRequestBuilder
{
    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="protoDefinition">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="protoDefinition">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matcher">The matcher to use to match the ProtoBuf as (json) object.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBodyAsProtoBuf(string protoDefinition, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBodyAsProtoBuf(string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matcher">The matcher to use to match the ProtoBuf as (json) object.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBodyAsProtoBuf(string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);
}