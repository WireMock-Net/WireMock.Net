// Copyright © WireMock.Net

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
        return WithBodyAsGraphQL(schema, matchBehaviour);
    }

    /// <inheritdoc />
    public IRequestBuilder WithGraphQLSchema(string schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsGraphQL(schema, customScalars, matchBehaviour);
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsGraphQL(string schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema));
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsGraphQL(string schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema, customScalars));
    }

#if GRAPHQL
    /// <inheritdoc />
    public IRequestBuilder WithGraphQLSchema(GraphQL.Types.ISchema schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsGraphQL(schema, matchBehaviour);
    }

    /// <inheritdoc />
    public IRequestBuilder WithGraphQLSchema(GraphQL.Types.ISchema schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsGraphQL(schema, customScalars, matchBehaviour);
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsGraphQL(GraphQL.Types.ISchema schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema));
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsGraphQL(GraphQL.Types.ISchema schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema, customScalars));
    }
#endif
}