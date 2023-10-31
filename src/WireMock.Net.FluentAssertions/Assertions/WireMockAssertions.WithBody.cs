#pragma warning disable CS1591

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public partial class WireMockAssertions
{
    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBody(string body, string because = "", params object[] becauseArgs)
    {
        bool Predicate(IRequestMessage request) => (body == Any && !string.IsNullOrEmpty(request.Body)) || request.Body == body;

        var (filter, condition) = BuildFilterAndCondition(Predicate);

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called using string body {0}{reason}, but no calls were made.",
                body
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called using string body {0}{reason}, but didn't find it among the string body {1}.",
                _ => body,
                requests => requests.Select(request => request.Body)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithBodyAsJson(object body, string because = "", params object[] becauseArgs)
    {
        bool Predicate(IRequestMessage request) => (body == Any && !string.IsNullOrEmpty(request.Body)) || request.Body == body;

        var (filter, condition) = BuildFilterAndCondition(Predicate);

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => _requestMessages)
            .ForCondition(requests => _callsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called using json body {0}{reason}, but no calls were made.",
                body
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called using json body {0}{reason}, but didn't find it among the json body {1}.",
                _ => body,
                requests => requests.Select(request => request.Body)
            );

        _requestMessages = filter(_requestMessages).ToList();

        return new AndConstraint<WireMockAssertions>(this);
    }
}