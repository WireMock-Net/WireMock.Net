#pragma warning disable CS1591
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public class WireMockAssertions
{
    private readonly IWireMockServer _subject;
    private readonly int? _callsCount;

    public WireMockAssertions(IWireMockServer subject, int? callsCount)
    {
        _subject = subject;
        _callsCount = callsCount;
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> AtAbsoluteUrl(string absoluteUrl, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _subject.LogEntries.Select(x => x.RequestMessage).ToList())
            .ForCondition(requests => requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but no calls were made.",
                absoluteUrl)
            .Then
            .ForCondition(x => _callsCount == null && x.Any(y => y.AbsoluteUrl == absoluteUrl) || _callsCount == x.Count(y => y.AbsoluteUrl == absoluteUrl))
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but didn't find it among the calls to {1}.",
                _ => absoluteUrl, requests => requests.Select(request => request.AbsoluteUrl));

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> AtUrl(string url, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _subject.LogEntries.Select(x => x.RequestMessage).ToList())
            .ForCondition(requests => requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the url {0}{reason}, but no calls were made.",
                url)
            .Then
            .ForCondition(x => _callsCount == null && x.Any(y => y.Url == url) || _callsCount == x.Count(y => y.Url == url))
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the url {0}{reason}, but didn't find it among the calls to {1}.",
                _ => url, requests => requests.Select(request => request.Url));

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithProxyUrl(string proxyUrl, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _subject.LogEntries.Select(x => x.RequestMessage).ToList())
            .ForCondition(requests => requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called with proxy url {0}{reason}, but no calls were made.",
                proxyUrl)
            .Then
            .ForCondition(x => _callsCount == null && x.Any(y => y.ProxyUrl == proxyUrl) || _callsCount == x.Count(y => y.ProxyUrl == proxyUrl))
            .FailWith(
                "Expected {context:wiremockserver} to have been called with proxy url {0}{reason}, but didn't find it among the calls with {1}.",
                _ => proxyUrl, requests => requests.Select(request => request.ProxyUrl));

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> FromClientIP(string clientIP, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _subject.LogEntries.Select(x => x.RequestMessage).ToList())
            .ForCondition(requests => requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called from client IP {0}{reason}, but no calls were made.",
                clientIP)
            .Then
            .ForCondition(x => _callsCount == null && x.Any(y => y.ClientIP == clientIP) || _callsCount == x.Count(y => y.ClientIP == clientIP))
            .FailWith(
                "Expected {context:wiremockserver} to have been called from client IP {0}{reason}, but didn't find it among the calls from IP(s) {1}.",
                _ => clientIP, requests => requests.Select(request => request.ClientIP));

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithHeader(string expectedKey, string value, string because = "", params object[] becauseArgs)
        => WithHeader(expectedKey, new[] { value }, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithHeader(string expectedKey, string[] expectedValues, string because = "", params object[] becauseArgs)
    {
        var headersDictionary = _subject.LogEntries.SelectMany(x => x.RequestMessage.Headers)
            .ToDictionary(x => x.Key, x => x.Value);

        using (new AssertionScope("headers from requests sent"))
        {
            headersDictionary.Should().ContainKey(expectedKey, because, becauseArgs);
        }

        using (new AssertionScope($"header \"{expectedKey}\" from requests sent with value(s)"))
        {
            if (expectedValues.Length == 1)
            {
                headersDictionary[expectedKey].Should().Contain(expectedValues.First());
            }
            else
            {
                var trimmedHeaderValues = string.Join(",", headersDictionary[expectedKey].Select(x => x)).Split(',')
                    .Select(x => x.Trim())
                    .ToList();
                foreach (var expectedValue in expectedValues)
                {
                    trimmedHeaderValues.Should().Contain(expectedValue);
                }
            }
        }

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingConnect(string because = "", params object[] becauseArgs)
        => UsingMethod("CONNECT", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingDelete(string because = "", params object[] becauseArgs)
        => UsingMethod("DELETE", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingGet(string because = "", params object[] becauseArgs)
        => UsingMethod("GET", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingHead(string because = "", params object[] becauseArgs)
        => UsingMethod("HEAD", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingOptions(string because = "", params object[] becauseArgs)
        => UsingMethod("OPTIONS", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingPost(string because = "", params object[] becauseArgs)
        => UsingMethod("POST", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingPatch(string because = "", params object[] becauseArgs)
        => UsingMethod("PATCH", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingPut(string because = "", params object[] becauseArgs)
        => UsingMethod("PUT", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingTrace(string because = "", params object[] becauseArgs)
        => UsingMethod("TRACE", because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingMethod(string method, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _subject.LogEntries.Select(x => x.RequestMessage).ToList())
            .ForCondition(requests => requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called using method {0}{reason}, but no calls were made.",
                method)
            .Then
            .ForCondition(x => _callsCount == null && x.Any(y => y.Method == method) || _callsCount == x.Count(y => y.Method == method))
            .FailWith(
                "Expected {context:wiremockserver} to have been called using method {0}{reason}, but didn't find it among the methods {1}.",
                _ => method, requests => requests.Select(request => request.Method));

        return new AndConstraint<WireMockAssertions>(this);
    }
}