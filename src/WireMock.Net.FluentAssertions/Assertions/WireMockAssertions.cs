using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions
{
    public class WireMockAssertions
    {
        private readonly IWireMockServer _subject;

        public WireMockAssertions(IWireMockServer subject, int? callsCount)
        {
            _subject = subject;
        }

        [CustomAssertion]
        public AndConstraint<WireMockAssertions> AtAbsoluteUrl(string absoluteUrl, string because = "",
            params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => _subject.LogEntries.Select(x => x.RequestMessage).ToList())
                .ForCondition(requests => requests.Any())
                .FailWith(
                    "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but no calls were made.",
                    absoluteUrl)
                .Then
                .ForCondition(x => x.Any(y => y.AbsoluteUrl == absoluteUrl))
                .FailWith(
                    "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but didn't find it among the calls to {1}.",
                    _ => absoluteUrl, requests => requests.Select(request => request.AbsoluteUrl));

            return new AndConstraint<WireMockAssertions>(this);
        }

        [CustomAssertion]
        public AndConstraint<WireMockAssertions> WithHeader(string expectedKey, string value,
            string because = "", params object[] becauseArgs)
            => WithHeader(expectedKey, new[] {value}, because, becauseArgs);

        [CustomAssertion]
        public AndConstraint<WireMockAssertions> WithHeader(string expectedKey, string[] expectedValues,
            string because = "", params object[] becauseArgs)
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
    }
}