// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Matchers.Request;

namespace WireMock.Owin;

internal class MappingMatcherResult
{
    public IMapping Mapping { get; }

    public IRequestMatchResult RequestMatchResult { get; }

    public MappingMatcherResult(IMapping mapping, IRequestMatchResult requestMatchResult)
    {
        Mapping = Guard.NotNull(mapping);
        RequestMatchResult = Guard.NotNull(requestMatchResult);
    }
}