using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;
using static System.Environment;

namespace WireMock.Net.Tests.FluentAssertions;

public class WireMockAssertionsTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly HttpClient _httpClient;
    private readonly int _portUsed;

    public WireMockAssertionsTests()
    {
        _server = WireMockServer.Start();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithSuccess());
        _portUsed = _server.Ports.First();

        _httpClient = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };
    }

    [Fact]
    public async Task HaveReceivedNoCalls_AtAbsoluteUrl_WhenACallWasNotMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx").ConfigureAwait(false);

        _server.Should()
            .HaveReceivedNoCalls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived0Calls_AtAbsoluteUrl_WhenACallWasNotMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx").ConfigureAwait(false);

        _server.Should()
            .HaveReceived(0).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived1Calls_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl").ConfigureAwait(false);

        _server.Should()
            .HaveReceived(1).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived2Calls_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl").ConfigureAwait(false);

        await _httpClient.GetAsync("anyurl").ConfigureAwait(false);

        _server.Should()
            .HaveReceived(2).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl").ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public void HaveReceivedACall_AtAbsoluteUrl_Should_ThrowWhenNoCallsWereMade()
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
    public async Task HaveReceivedACall_AtAbsoluteUrl_Should_ThrowWhenNoCallsMatchingTheAbsoluteUrlWereMade()
    {
        await _httpClient.GetAsync("").ConfigureAwait(false);

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl("anyurl");

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                $"Expected _server to have been called at address matching the absolute url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{_portUsed}/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_WhenACallWasMadeWithExpectedHeader_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer a");
        await _httpClient.GetAsync("").ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .WithHeader("Authorization", "Bearer a");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_WhenACallWasMadeWithExpectedHeaderAmongMultipleHeaderValues_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        await _httpClient.GetAsync("").ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .WithHeader("Accept", new[] { "application/xml", "application/json" });
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderNameWereMade()
    {
        await _httpClient.GetAsync("").ConfigureAwait(false);

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .WithHeader("Authorization", "value");

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Contain("to contain key \"Authorization\".");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderValuesWereMade()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        await _httpClient.GetAsync("").ConfigureAwait(false);

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
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderWithMultipleValuesWereMade()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        await _httpClient.GetAsync("").ConfigureAwait(false);

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

    [Fact]
    public async Task HaveReceivedACall_AtUrl_WhenACallWasMadeToUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl").ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .AtUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public void HaveReceivedACall_AtUrl_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtUrl("anyurl");

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                "Expected _server to have been called at address matching the url \"anyurl\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_AtUrl_Should_ThrowWhenNoCallsMatchingTheUrlWereMade()
    {
        await _httpClient.GetAsync("").ConfigureAwait(false);

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtUrl("anyurl");

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                $"Expected _server to have been called at address matching the url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{_portUsed}/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithProxyUrl_WhenACallWasMadeWithProxyUrl_Should_BeOK()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        await _httpClient.GetAsync("").ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .WithProxyUrl($"http://localhost:9999");
    }

    [Fact]
    public void HaveReceivedACall_WithProxyUrl_Should_ThrowWhenNoCallsWereMade()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .WithProxyUrl("anyurl");

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                "Expected _server to have been called with proxy url \"anyurl\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithProxyUrl_Should_ThrowWhenNoCallsWithTheProxyUrlWereMade()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        await _httpClient.GetAsync("").ConfigureAwait(false);

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .WithProxyUrl("anyurl");

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                $"Expected _server to have been called with proxy url \"anyurl\", but didn't find it among the calls with {{\"http://localhost:9999\"}}.");
    }

    [Fact]
    public async Task HaveReceivedACall_FromClientIP_whenACallWasMadeFromClientIP_Should_BeOK()
    {
        await _httpClient.GetAsync("").ConfigureAwait(false);
        var clientIP = _server.LogEntries.Last().RequestMessage.ClientIP;

        _server.Should()
            .HaveReceivedACall()
            .FromClientIP(clientIP);
    }

    [Fact]
    public void HaveReceivedACall_FromClientIP_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _server.Should()
            .HaveReceivedACall()
            .FromClientIP("different-ip");

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                "Expected _server to have been called from client IP \"different-ip\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_FromClientIP_Should_ThrowWhenNoCallsFromClientIPWereMade()
    {
        await _httpClient.GetAsync("").ConfigureAwait(false);
        var clientIP = _server.LogEntries.Last().RequestMessage.ClientIP;

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .FromClientIP("different-ip");

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                $"Expected _server to have been called from client IP \"different-ip\", but didn't find it among the calls from IP(s) {{\"{clientIP}\"}}.");
    }

    public void Dispose()
    {
        _server?.Stop();
        _server?.Dispose();
        _httpClient?.Dispose();
    }
}