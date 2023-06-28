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
}