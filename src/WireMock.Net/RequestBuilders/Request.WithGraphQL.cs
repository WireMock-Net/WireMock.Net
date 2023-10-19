using System.Collections.Generic;
using System;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithGraphQLSchema(string schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithGraphQLSchema(string schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema, customScalars));
        return this;
    }

#if GRAPHQL
    /// <inheritdoc />
    public IRequestBuilder WithGraphQLSchema(GraphQL.Types.ISchema schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithGraphQLSchema(GraphQL.Types.ISchema schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema, customScalars));
        return this;
    }
#endif
}