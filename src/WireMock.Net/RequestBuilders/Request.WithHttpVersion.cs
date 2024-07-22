// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithHttpVersion(string version, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Add(new RequestMessageHttpVersionMatcher(matchBehaviour, version));
    }
}