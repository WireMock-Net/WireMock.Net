#pragma warning disable CS1591
using System;
using System.Collections.Generic;
using WireMock.Matchers;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public partial class WireMockAssertions
{
    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBody(string body, string because = "", params object[] becauseArgs)
    {
        return WithBody(new WildcardMatcher(body), because, becauseArgs);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBodyAsJson(object body, string because = "", params object[] becauseArgs)
    {
        return WithBodyAsJson(new JsonMatcher(body), because, becauseArgs);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBodyAsJson(string body, string because = "", params object[] becauseArgs)
    {
        return WithBodyAsJson(new JsonMatcher(body), because, becauseArgs);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBodyAsBytes(byte[] body, string because = "", params object[] becauseArgs)
    {
        return WithBodyAsBytes(new ExactObjectMatcher(body), because, becauseArgs);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBody(IStringMatcher matcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(r => r.Body, matcher);

        return ExecuteAssertionWithBodyStringMatcher(matcher, because, becauseArgs, condition, filter, r => r.Body);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBodyAsJson(IValueMatcher matcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(r => r.BodyAsJson, matcher);

        return ExecuteAssertionWithBodyAsJsonValueMatcher(matcher, because, becauseArgs, condition, filter, r => r.BodyAsJson);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBodyAsBytes(ExactObjectMatcher matcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(r => r.BodyAsBytes, matcher);

        return ExecuteAssertionWithBodyExactObjectMatcher(matcher, because, becauseArgs, condition, filter, r => r.BodyAsBytes);
    }

    private AndConstraint<WireMockAssertions> ExecuteAssertionWithBodyStringMatcher(
        IStringMatcher matcher,
        string because,
        object[] becauseArgs,
        Func<IReadOnlyList<IRequestMessage>, bool> condition,
        Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> filter,
        Func<IRequestMessage, object?> expression
    )
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called using body matcher {0}{reason}, but no calls were made.",
                matcher.GetPatterns()
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called using body matcher {0}{reason}, but didn't find it among the body {1}.",
                _ => matcher.GetPatterns(),
                requests => requests.Select(expression)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }

    private AndConstraint<WireMockAssertions> ExecuteAssertionWithBodyAsJsonValueMatcher(
        IValueMatcher matcher,
        string because,
        object[] becauseArgs,
        Func<IReadOnlyList<IRequestMessage>, bool> condition,
        Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> filter,
        Func<IRequestMessage, object?> expression
    )
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called using body {0}{reason}, but no calls were made.",
                matcher.Value
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called using body {0}{reason}, but didn't find it among the body {1}.",
                _ => matcher.Value,
                requests => requests.Select(expression)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }

    private AndConstraint<WireMockAssertions> ExecuteAssertionWithBodyExactObjectMatcher(
        ExactObjectMatcher matcher,
        string because,
        object[] becauseArgs,
        Func<IReadOnlyList<IRequestMessage>, bool> condition,
        Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> filter,
        Func<IRequestMessage, object?> expression
    )
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called using body {0}{reason}, but no calls were made.",
                matcher.ValueAsObject ?? matcher.ValueAsBytes
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called using body {0}{reason}, but didn't find it among the body {1}.",
                _ => matcher.ValueAsObject ?? matcher.ValueAsBytes,
                requests => requests.Select(expression)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }
}