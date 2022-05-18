// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System.Linq;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Stef.Validation;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder UsingConnect(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.CONNECT));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingDelete(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.DELETE));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingGet(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.GET));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingHead(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.HEAD));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingOptions(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.OPTIONS));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingPost(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.POST));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingPatch(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.PATCH));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingPut(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.PUT));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingTrace(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, MatchOperator.Or, HttpRequestMethods.TRACE));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingAnyMethod()
    {
        var matchers = _requestMatchers.Where(m => m is RequestMessageMethodMatcher).ToList();
        foreach (var matcher in matchers)
        {
            _requestMatchers.Remove(matcher);
        }

        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder UsingMethod(params string[] methods)
    {
        return UsingMethod(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, methods);
    }

    /// <inheritdoc />
    public IRequestBuilder UsingMethod(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params string[] methods)
    {
        Guard.NotNullOrEmpty(methods);

        _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, matchOperator, methods));
        return this;
    }
}