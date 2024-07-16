// Copyright © WireMock.Net

#pragma warning disable CS1591
using System;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public partial class WireMockAssertions
{
    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> AtAbsoluteUrl(string absoluteUrl, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.AbsoluteUrl, absoluteUrl, StringComparison.OrdinalIgnoreCase));

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
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

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAssertions, string>(this, absoluteUrl);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> AtUrl(string url, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.Url, url, StringComparison.OrdinalIgnoreCase));

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
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

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAssertions, string>(this, url);
    }
}