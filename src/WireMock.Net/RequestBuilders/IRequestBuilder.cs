using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilder
/// </summary>
public interface IRequestBuilder : IClientIPRequestBuilder
{
    /// <summary>
    /// The link back to the Mapping.
    /// </summary>
    internal IMapping Mapping { get; set; }

    internal IRequestBuilder Add<T>(T requestMatcher) where T : IRequestMatcher;
}