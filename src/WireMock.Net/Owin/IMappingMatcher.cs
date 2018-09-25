using WireMock.Matchers.Request;

namespace WireMock.Owin
{
    internal interface IMappingMatcher
    {
        (IMapping Mapping, RequestMatchResult RequestMatchResult) Match(RequestMessage request);
    }
}