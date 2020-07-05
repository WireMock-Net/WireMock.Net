using WireMock.Matchers.Request;

namespace WireMock.Owin
{
    internal class MappingMatcherResult
    {
        public IMapping Mapping { get; set; }

        public IRequestMatchResult RequestMatchResult { get; set; }
    }
}