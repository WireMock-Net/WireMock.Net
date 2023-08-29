using WireMock.Matchers;

namespace WireMock.RequestBuilders;

/// <summary>
/// The GraphQLRequestBuilder interface.
/// </summary>
public interface IGraphQLRequestBuilder : IMultiPartRequestBuilder
{
    /// <summary>
    /// WithGraphQLSchema: The GraphQL schema as a string.
    /// </summary>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithGraphQLSchema(string schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

#if GRAPHQL
    /// <summary>
    /// WithGraphQLSchema: The GraphQL schema as a ISchema.
    /// </summary>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithGraphQLSchema(GraphQL.Types.ISchema schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);
#endif
}