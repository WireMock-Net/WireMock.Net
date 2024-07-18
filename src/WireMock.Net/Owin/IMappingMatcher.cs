// Copyright © WireMock.Net

namespace WireMock.Owin;

internal interface IMappingMatcher
{
    (MappingMatcherResult? Match, MappingMatcherResult? Partial) FindBestMatch(RequestMessage request);
}