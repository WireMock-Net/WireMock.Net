// ReSharper disable InconsistentNaming
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilderExtensions extensions for GraphQL.
/// </summary>
public static class IRequestBuilderExtensions
{
    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="protoDefinition">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, string protoDefinition, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => new(null, protoDefinition), messageType));
    }

    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="protoDefinition">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matcher">The matcher to use to match the ProtoBuf as (json) object.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, string protoDefinition, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => new(null, protoDefinition), messageType, matcher));
    }

    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => requestBuilder.Mapping.ProtoDefinition!.Value, messageType));
    }

    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matcher">The matcher to use to match the ProtoBuf as (json) object.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => requestBuilder.Mapping.ProtoDefinition!.Value, messageType, matcher));
    }
}