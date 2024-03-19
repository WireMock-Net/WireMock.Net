#pragma warning disable CS1591
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Matchers;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public partial class WireMockAssertions
{
    private const string MessageFormatNoCalls = "Expected {context:wiremockserver} to have been called using body {0}{reason}, but no calls were made.";
    private const string MessageFormat = "Expected {context:wiremockserver} to have been called using body {0}{reason}, but didn't find it among the body/bodies {1}.";

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
    public AndConstraint<WireMockAssertions> WithBodyAsJson(IObjectMatcher matcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(r => r.BodyAsJson, matcher);

        return ExecuteAssertionWithBodyAsIObjectMatcher(matcher, because, becauseArgs, condition, filter, r => r.BodyAsJson);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBodyAsBytes(ExactObjectMatcher matcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(r => r.BodyAsBytes, matcher);

        return ExecuteAssertionWithBodyAsBytesExactObjectMatcher(matcher, because, becauseArgs, condition, filter, r => r.BodyAsBytes);
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
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                MessageFormatNoCalls,
                matcher.GetPatterns()
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                MessageFormat,
                _ => matcher.GetPatterns(),
                requests => requests.Select(expression)
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAssertions>(this);
    }

    private AndConstraint<WireMockAssertions> ExecuteAssertionWithBodyAsIObjectMatcher(
        IObjectMatcher matcher,
        string because,
        object[] becauseArgs,
        Func<IReadOnlyList<IRequestMessage>, bool> condition,
        Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> filter,
        Func<IRequestMessage, object?> expression
    )
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                MessageFormatNoCalls,
                FormatBody(matcher.Value)
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                MessageFormat,
                _ => FormatBody(matcher.Value),
                requests => FormatBodies(requests.Select(expression))
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAssertions>(this);
    }

    private AndConstraint<WireMockAssertions> ExecuteAssertionWithBodyAsBytesExactObjectMatcher(
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
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                MessageFormatNoCalls,
                matcher.Value
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                MessageFormat,
                _ => matcher.Value,
                requests => requests.Select(expression)
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAssertions>(this);
    }

    private static string? FormatBody(object? body)
    {
        return body switch
        {
            null => null,
            JObject jObject => jObject.ToString(Formatting.None),
            JToken jToken => jToken.ToString(Formatting.None),
            _ => JToken.FromObject(body).ToString(Formatting.None)
        };
    }

    private static string? FormatBodies(IEnumerable<object?> bodies)
    {
        return string.Join(", ", bodies.Select(FormatBody));
    }
}