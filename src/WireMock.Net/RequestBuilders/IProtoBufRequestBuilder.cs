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
    /// <param name="protoDefinition">The proto definition as a string.</param>
    /// <param name="grpcServiceMethod">The method which is called on service. Format is "{package-name}.{service-name}-{method-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithGrpcProto(string protoDefinition, string grpcServiceMethod, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithGrpcProto
    /// </summary>
    /// <param name="protoDefinition">The proto definition as a string.</param>
    /// <param name="grpcServiceMethod">The method which is called on service. Format is "{package-name}.{service-name}-{method-name}".</param>
    /// <param name="jsonMatcher">The jsonMatcher to use to match the ProtoBuf as (json) object.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithGrpcProto(string protoDefinition, string grpcServiceMethod, IJsonMatcher jsonMatcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);
}