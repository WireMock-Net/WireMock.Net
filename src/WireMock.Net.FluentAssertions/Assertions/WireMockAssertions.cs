#pragma warning disable CS1591
using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public partial class WireMockAssertions
{
    private const string Any = "*";
    private readonly int? _callsCount;
    private IReadOnlyList<IRequestMessage> _requestMessages;
    //private readonly IReadOnlyList<KeyValuePair<string, WireMockList<string>>> _headers;

    public WireMockAssertions(IWireMockServer subject, int? callsCount)
    {
        _callsCount = callsCount;
        _requestMessages = subject.LogEntries.Select(logEntry => logEntry.RequestMessage).ToList();
        // _headers = _requestMessages.SelectMany(req => req.Headers).ToList();
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> AtAbsoluteUrl(string absoluteUrl, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.AbsoluteUrl, absoluteUrl, StringComparison.OrdinalIgnoreCase));

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but no calls were made.",
                absoluteUrl
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but didn't find it among the calls to {1}.",
                _ => absoluteUrl,
                requests => requests.Select(request => request.AbsoluteUrl)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndWhichConstraint<WireMockAssertions, string>(this, absoluteUrl);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> AtUrl(string url, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.Url, url, StringComparison.OrdinalIgnoreCase));

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the url {0}{reason}, but no calls were made.",
                url
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the url {0}{reason}, but didn't find it among the calls to {1}.",
                _ => url,
                requests => requests.Select(request => request.Url)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndWhichConstraint<WireMockAssertions, string>(this, url);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> WithProxyUrl(string proxyUrl, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.ProxyUrl, proxyUrl, StringComparison.OrdinalIgnoreCase));

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called with proxy url {0}{reason}, but no calls were made.",
                proxyUrl
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called with proxy url {0}{reason}, but didn't find it among the calls with {1}.",
                _ => proxyUrl,
                requests => requests.Select(request => request.ProxyUrl)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndWhichConstraint<WireMockAssertions, string>(this, proxyUrl);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> FromClientIP(string clientIP, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.ClientIP, clientIP, StringComparison.OrdinalIgnoreCase));

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called from client IP {0}{reason}, but no calls were made.",
                clientIP
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called from client IP {0}{reason}, but didn't find it among the calls from IP(s) {1}.",
                _ => clientIP, requests => requests.Select(request => request.ClientIP)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndWhichConstraint<WireMockAssertions, string>(this, clientIP);
    }

    private (Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> Filter, Func<IReadOnlyList<IRequestMessage>, bool> Condition) BuildFilterAndCondition(Func<IRequestMessage, bool> predicate)
    {
        Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> filter = requests => requests.Where(predicate).ToList();

        return (filter, requests => (_callsCount is null && filter(requests).Any()) || _callsCount == filter(requests).Count);
    }

    private (Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> Filter, Func<IReadOnlyList<IRequestMessage>, bool> Condition) BuildFilterAndCondition(Func<IRequestMessage, string?> expression, IStringMatcher matcher)
    {
        return BuildFilterAndCondition(r => matcher.IsMatch(expression(r)).IsPerfect());
    }

    private (Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> Filter, Func<IReadOnlyList<IRequestMessage>, bool> Condition) BuildFilterAndCondition(Func<IRequestMessage, object?> expression, IObjectMatcher matcher)
    {
        return BuildFilterAndCondition(r => matcher.IsMatch(expression(r)).IsPerfect());
    }
}