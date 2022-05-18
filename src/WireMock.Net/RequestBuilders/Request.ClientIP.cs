using System;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithClientIP(params IStringMatcher[] matchers)
    {
        return WithClientIP(MatchOperator.Or, matchers);
    }

    /// <inheritdoc />
    public IRequestBuilder WithClientIP(MatchOperator matchOperator, params IStringMatcher[] matchers)
    {
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageClientIPMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithClientIP(params string[] paths)
    {
        return WithClientIP(MatchOperator.Or, paths);
    }

    /// <inheritdoc />
    public IRequestBuilder WithClientIP(MatchOperator matchOperator, params string[] paths)
    {
        Guard.NotNullOrEmpty(paths);

        _requestMatchers.Add(new RequestMessageClientIPMatcher(MatchBehaviour.AcceptOnMatch, matchOperator, paths));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithClientIP(params Func<string, bool>[] funcs)
    {
        Guard.NotNullOrEmpty(funcs);

        _requestMatchers.Add(new RequestMessageClientIPMatcher(funcs));
        return this;
    }
}