using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilder
/// </summary>
public interface IRequestBuilder : IClientIPRequestBuilder
{
    internal IRequestBuilder Add<T>(T requestMatcher) where T : IRequestMatcher;
}