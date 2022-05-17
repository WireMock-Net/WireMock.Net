using System;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithPath(params IStringMatcher[] matchers)
    {
        return WithPath(MatchOperator.Or, matchers);
    }

    /// <inheritdoc />
    public IRequestBuilder WithPath(MatchOperator matchOperator, params IStringMatcher[] matchers)
    {
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessagePathMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithPath(params string[] paths)
    {
        return WithPath(MatchOperator.Or, paths);
    }

    ///// <inheritdoc cref="IUrlAndPathRequestBuilder.WithPath(MatchBehaviour, string[])"/>
    //public IRequestBuilder WithPath(MatchBehaviour matchBehaviour, params string[] paths)
    //{
    //    Guard.NotNullOrEmpty(paths);

    //    _requestMatchers.Add(new RequestMessagePathMatcher(matchBehaviour, MatchOperator.Or, paths));
    //    return this;
    //}

    /// <inheritdoc />
    public IRequestBuilder WithPath(MatchOperator matchOperator, params string[] paths)
    {
        Guard.NotNullOrEmpty(paths);

        _requestMatchers.Add(new RequestMessagePathMatcher(MatchBehaviour.AcceptOnMatch, matchOperator, paths));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithPath(params Func<string, bool>[] funcs)
    {
        Guard.NotNullOrEmpty(funcs);

        _requestMatchers.Add(new RequestMessagePathMatcher(funcs));
        return this;
    }
}