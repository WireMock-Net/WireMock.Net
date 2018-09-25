using WireMock.Matchers.Request;

namespace WireMock.Owin
{
    internal interface IMappingMatcher
    {
        (Mapping Mapping, RequestMatchResult RequestMatchResult) Match(RequestMessage request);
    }
}