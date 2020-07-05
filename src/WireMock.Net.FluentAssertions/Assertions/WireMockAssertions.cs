using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions
{
    public class WireMockAssertions
    {
        private readonly IWireMockServer _instance;

        public WireMockAssertions(IWireMockServer instance, int? callsCount)
        {
            _instance = instance;
        }

        [CustomAssertion]
        public AndConstraint<WireMockAssertions> AtAbsoluteUrl(string absoluteUrl, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => _instance.LogEntries.Select(x => x.RequestMessage).ToList())
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
    }
}