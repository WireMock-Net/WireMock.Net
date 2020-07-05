using FluentAssertions;
using System;
using System.Net.Http;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using WireMock.FluentAssertions;
using System.Threading.Tasks;

namespace WireMock.Net.Tests.FluentAssertions
{
    public class WireMockAssertionsTests : IDisposable
    {
        private WireMockServer _server;
        private HttpClient _httpClient;
        private const int Port = 42000;

        public WireMockAssertionsTests()
        {
            _server = WireMockServer.Start(Port);
            _server.Given(Request.Create().UsingAnyMethod())
                            .RespondWith(Response.Create().WithStatusCode(200));

            _httpClient = new HttpClient { BaseAddress = new Uri($"http://localhost:{Port}") };
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
                    $"Expected _server to have been called at address matching the absolute url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{Port}/\"}}.");
        }

        public void Dispose()
        {
            _server?.Stop();
            _server?.Dispose();
        }
    }
}
