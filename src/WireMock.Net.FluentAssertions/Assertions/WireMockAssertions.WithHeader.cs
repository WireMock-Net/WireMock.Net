#pragma warning disable CS1591
using System.Collections.Generic;
using WireMock.Types;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public partial class WireMockAssertions
{
    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WitHeaderKey(string expectedKey, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request =>
        {
            return request.Headers?.Any(h => h.Key == expectedKey) == true;
        });

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called with Header {0}{reason}.",
                expectedKey
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called with Header {0}{reason}, but didn't find it among the calls with Header(s) {1}.",
                _ => expectedKey,
                requests => requests.Select(request => request.Headers)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithHeader(string expectedKey, string value, string because = "", params object[] becauseArgs)
        => WithHeader(expectedKey, new[] { value }, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithHeader(string expectedKey, string[] expectedValues, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request =>
        {
            var headers = request.Headers?.ToArray() ?? new KeyValuePair<string, WireMockList<string>>[0];

            var matchingHeaderValues = headers.Where(h => h.Key == expectedKey).SelectMany(h => h.Value.ToArray()).ToArray();

            if (expectedValues.Length == 1 && matchingHeaderValues.Length == 1)
            {
                return matchingHeaderValues[0] == expectedValues[0];
            }

            var trimmedHeaderValues = string.Join(",", matchingHeaderValues.Select(x => x)).Split(',').Select(x => x.Trim()).ToArray();
            return expectedValues.Any(trimmedHeaderValues.Contains);
        });

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called with Header {0} and Values {1}{reason}.",
                expectedKey,
                expectedValues
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called with Header {0} and Values {1}{reason}, but didn't find it among the calls with Header(s) {2}.",
                _ => expectedKey,
                _ => expectedValues,
                requests => requests.Select(request => request.Headers)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithoutHeaderKey(string unexpectedKey, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request =>
        {
            return request.Headers?.Any(h => h.Key == unexpectedKey) != true;
        });

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} not to have been called with Header {0}{reason}.",
                unexpectedKey
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} not to have been called with Header {0}{reason}, but found it among the calls with Header(s) {1}.",
                _ => unexpectedKey,
                requests => requests.Select(request => request.Headers)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithoutHeader(string unexpectedKey, string value, string because = "", params object[] becauseArgs)
        => WithoutHeader(unexpectedKey, new[] { value }, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithoutHeader(string unexpectedKey, string[] expectedValues, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request =>
        {
            var headers = request.Headers?.ToArray() ?? new KeyValuePair<string, WireMockList<string>>[0];

            var matchingHeaderValues = headers.Where(h => h.Key == unexpectedKey).SelectMany(h => h.Value.ToArray()).ToArray();

            if (expectedValues.Length == 1 && matchingHeaderValues.Length == 1)
            {
                return matchingHeaderValues[0] != expectedValues[0];
            }

            var trimmedHeaderValues = string.Join(",", matchingHeaderValues.Select(x => x)).Split(',').Select(x => x.Trim()).ToArray();
            return !expectedValues.Any(trimmedHeaderValues.Contains);
        });

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} not to have been called with Header {0} and Values {1}{reason}.",
                unexpectedKey,
                expectedValues
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} not to have been called with Header {0} and Values {1}{reason}, but found it among the calls with Header(s) {2}.",
                _ => unexpectedKey,
                _ => expectedValues,
                requests => requests.Select(request => request.Headers)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }
}