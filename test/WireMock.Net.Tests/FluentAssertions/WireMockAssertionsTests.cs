using FluentAssertions;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using WireMock.FluentAssertions;
using System.Threading.Tasks;
using static System.Environment;

namespace WireMock.Net.Tests.FluentAssertions
{
    public class WireMockAssertionsTests : IDisposable
    {
        private WireMockServer _server;
        private HttpClient _httpClient;
        private int _portUsed;

        public WireMockAssertionsTests()
        {
            _server = WireMockServer.Start();
            _server.Given(Request.Create().UsingAnyMethod())
                .RespondWith(Response.Create().WithStatusCode(200));
            _portUsed = _server.Ports.First();

            _httpClient = new HttpClient { BaseAddress = new Uri($"http://localhost:{_portUsed}") };
        }

        [Fact]
        public async Task AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
        {
            await _httpClient.GetAsync("anyurl");

            _server.Should()
                .HaveReceivedACall()
                .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
        }

        [Fact]
        public void AtAbsoluteUrl_Should_ThrowWhenNoCallsWereMade()
        {
            Action act = () => _server.Should()
                .HaveReceivedACall()
                .AtAbsoluteUrl("anyurl");

            act.Should().Throw<Exception>()
                .And.Message.Should()
                .Be(
                    "Expected _server to have been called at address matching the absolute url \"anyurl\", but no calls were made.");
        }

        [Fact]
        public async Task AtAbsoluteUrl_Should_ThrowWhenNoCallsMatchingTheAbsoluteUrlWereMade()
        {
            await _httpClient.GetAsync("");

            Action act = () => _server.Should()
                .HaveReceivedACall()
                .AtAbsoluteUrl("anyurl");

            act.Should().Throw<Exception>()
                .And.Message.Should()
                .Be(
                    $"Expected _server to have been called at address matching the absolute url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{_portUsed}/\"}}.");
        }

        [Fact]
        public async Task WithHeader_WhenACallWasMadeWithExpectedHeader_Should_BeOK()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer a");
            await _httpClient.GetAsync("");

            _server.Should()
                .HaveReceivedACall()
                .WithHeader("Authorization", "Bearer a");
        }

        [Fact]
        public async Task WithHeader_WhenACallWasMadeWithExpectedHeaderAmongMultipleHeaderValues_Should_BeOK()
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            await _httpClient.GetAsync("");

            _server.Should()
                .HaveReceivedACall()
                .WithHeader("Accept", new[] { "application/xml", "application/json" });
        }

        [Fact]
        public async Task WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderNameWereMade()
        {
            await _httpClient.GetAsync("");

            Action act = () => _server.Should()
                .HaveReceivedACall()
                .WithHeader("Authorization", "value");

            var sentHeaders = _server.LogEntries.SelectMany(x => x.RequestMessage.Headers)
                .ToDictionary(x => x.Key, x => x.Value)
                .ToList();

            var sentHeaderString = "{" + string.Join(", ", sentHeaders) + "}";

            act.Should().Throw<Exception>()
                .And.Message.Should()
                .Be(
                    $"Expected headers from requests sent {sentHeaderString} to contain key \"Authorization\".{NewLine}");
        }

        [Fact]
        public async Task WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderValuesWereMade()
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            await _httpClient.GetAsync("");

            Action act = () => _server.Should()
                .HaveReceivedACall()
                .WithHeader("Accept", "missing-value");

            var sentHeaders = _server.LogEntries.SelectMany(x => x.RequestMessage.Headers)
                .ToDictionary(x => x.Key, x => x.Value)["Accept"]
                .Select(x => $"\"{x}\"")
                .ToList();

            var sentHeaderString = "{" + string.Join(", ", sentHeaders) + "}";

            act.Should().Throw<Exception>()
                .And.Message.Should()
                .Be(
                    $"Expected header \"Accept\" from requests sent with value(s) {sentHeaderString} to contain \"missing-value\".{NewLine}");
        }

        [Fact]
        public async Task WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderWithMultipleValuesWereMade()
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            await _httpClient.GetAsync("");

            Action act = () => _server.Should()
                .HaveReceivedACall()
                .WithHeader("Accept", new[] { "missing-value1", "missing-value2" });

            const string missingValue1Message =
                "Expected header \"Accept\" from requests sent with value(s) {\"application/xml\", \"application/json\"} to contain \"missing-value1\".";
            const string missingValue2Message =
                "Expected header \"Accept\" from requests sent with value(s) {\"application/xml\", \"application/json\"} to contain \"missing-value2\".";

            act.Should().Throw<Exception>()
                .And.Message.Should()
                .Be($"{string.Join(NewLine, missingValue1Message, missingValue2Message)}{NewLine}");
        }

        public void Dispose()
        {
            _server?.Stop();
            _server?.Dispose();
        }
    }
}