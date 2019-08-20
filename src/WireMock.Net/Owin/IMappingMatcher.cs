namespace WireMock.Owin
{
    internal interface IMappingMatcher
    {
        MappingMatcherResult FindBestMatch(RequestMessage request);
    }
}