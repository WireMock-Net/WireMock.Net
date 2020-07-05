using WireMock.Matchers.Request;

namespace WireMock.Owin
{
    internal class MappingMatcherResult
    {
        public IMapping Mapping { get; set; }

        public RequestMatchResult RequestMatchResult { get; set; }
    }
}