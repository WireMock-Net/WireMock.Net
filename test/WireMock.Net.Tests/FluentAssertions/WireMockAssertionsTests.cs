using System;
using System.Linq;
using System.Net;
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
        _server.Given(Request.Create().UsingAnyMethod()).RespondWith(Response.Create().WithSuccess());

        _portUsed = _server.Ports.First();

        _httpClient = new HttpClient { BaseAddress = new Uri(_server.Url!) };
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
    public async Task HaveReceived1Calls_AtAbsoluteUrlUsingPost_WhenAPostCallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.PostAsync("anyurl", new StringContent("")).ConfigureAwait(false);

        _server.Should()
            .HaveReceived(1).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl")
            .And
            .UsingPost();
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
    public async Task HaveReceivedACall_WithHeader_WhenMultipleCallsWereMadeWithExpectedHeaderAmongMultipleHeaderValues_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        await _httpClient.GetAsync("1").ConfigureAwait(false);

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("EN"));
        await _httpClient.GetAsync("2").ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .WithHeader("Accept", new[] { "application/xml", "application/json" })
            .And
            .WithHeader("Accept-Language", new[] { "EN" });
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
            .Contain("to contain \"Authorization\".");
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
    public async Task HaveReceivedACall_WithHeader_ShouldCheckAllRequests()
    {
        // Arrange
        using var server = WireMockServer.Start();
        using var client1 = server.CreateClient();

        var handler = new HttpClientHandler();
        using var client2 = server.CreateClient(handler);

        // Act 1
        await client1.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken")
            }
        });

        // Act 2
        await client2.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", "validToken")
            }
        });

        // Assert
        server.Should().HaveReceivedACall().WithHeader("Authorization", "Bearer invalidToken");
        server.Should().HaveReceivedACall().WithHeader("Authorization", "Bearer validToken");
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

    [Fact]
    public async Task HaveReceivedNoCalls_UsingPost_WhenACallWasNotMadeUsingPost_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl").ConfigureAwait(false);

        _server.Should()
            .HaveReceivedNoCalls()
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived2Calls_UsingDelete_WhenACallWasMadeUsingDelete_Should_BeOK()
    {
        await _httpClient.DeleteAsync("anyurl").ConfigureAwait(false);

        await _httpClient.DeleteAsync("anyurl").ConfigureAwait(false);

        await _httpClient.GetAsync("anyurl").ConfigureAwait(false);

        _server.Should()
            .HaveReceived(2).Calls()
            .UsingDelete();
    }

    [Fact]
    public void HaveReceivedACall_UsingPatch_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _server.Should()
            .HaveReceivedACall()
            .UsingPatch();

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                "Expected _server to have been called using method \"PATCH\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_UsingOptions_Should_ThrowWhenCallsWereNotMadeUsingOptions()
    {
        await _httpClient.PostAsync("anyurl", new StringContent("anycontent")).ConfigureAwait(false);

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .UsingOptions();

        act.Should().Throw<Exception>()
            .And.Message.Should()
            .Be(
                "Expected _server to have been called using method \"OPTIONS\", but didn't find it among the methods {\"POST\"}.");
    }

#if !NET452
    [Fact]
    public async Task HaveReceivedACall_UsingConnect_WhenACallWasMadeUsingConnect_Should_BeOK()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Found));

        _httpClient.DefaultRequestHeaders.Add("Host", new Uri(_server.Urls[0]).Authority);

        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("CONNECT"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingConnect();
    }
#endif

    [Fact]
    public async Task HaveReceivedACall_UsingDelete_WhenACallWasMadeUsingDelete_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("DELETE"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingDelete();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingGet_WhenACallWasMadeUsingGet_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("GET"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingGet();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingHead_WhenACallWasMadeUsingHead_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("HEAD"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingHead();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingOptions_WhenACallWasMadeUsingOptions_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("OPTIONS"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingOptions();
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("Post")]
    public async Task HaveReceivedACall_UsingPost_WhenACallWasMadeUsingPost_Should_BeOK(string method)
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(method), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived1Calls_AtAbsoluteUrlUsingPost_ShouldChain()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingGet())
            .RespondWith(Response.Create().WithBody("A response").WithStatusCode(HttpStatusCode.OK));

        server
            .Given(Request.Create().WithPath("/b").UsingPost())
            .RespondWith(Response.Create().WithBody("B response").WithStatusCode(HttpStatusCode.OK));

        server
            .Given(Request.Create().WithPath("/c").UsingPost())
            .RespondWith(Response.Create().WithBody("C response").WithStatusCode(HttpStatusCode.OK));

        // Act
        var httpClient = new HttpClient();

        await httpClient.GetAsync($"{server.Url}/a");

        await httpClient.PostAsync($"{server.Url}/b", new StringContent("B"));

        await httpClient.PostAsync($"{server.Url}/c", new StringContent("C"));

        // Assert
        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/a")
            .And
            .UsingGet();

        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/b")
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/c")
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(3)
            .Calls();

        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .UsingGet();

        server
            .Should()
            .HaveReceived(2)
            .Calls()
            .UsingPost();

        server.Stop();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingPatch_WhenACallWasMadeUsingPatch_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingPatch();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingPut_WhenACallWasMadeUsingPut_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PUT"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingPut();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingTrace_WhenACallWasMadeUsingTrace_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("TRACE"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingTrace();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingAnyMethod_WhenACallWasMadeUsingGet_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("GET"), "anyurl")).ConfigureAwait(false);

        _server.Should()
            .HaveReceivedACall()
            .UsingAnyMethod();
    }

    [Fact]
    public void HaveReceivedNoCalls_UsingAnyMethod_WhenNoCallsWereMade_Should_BeOK()
    {
        _server
            .Should()
            .HaveReceived(0)
            .Calls()
            .UsingAnyMethod();

        _server
            .Should()
            .HaveReceivedNoCalls()
            .UsingAnyMethod();
    }

    [Fact]
    public void HaveReceivedNoCalls_AtUrl_WhenNoCallsWereMade_Should_BeOK()
    {
        _server.Should()
            .HaveReceived(0)
            .Calls()
            .AtUrl(_server.Url ?? string.Empty);

        _server.Should()
            .HaveReceivedNoCalls()
            .AtUrl(_server.Url ?? string.Empty);
    }

    public void Dispose()
    {
        _server?.Stop();
        _server?.Dispose();
        _httpClient?.Dispose();
    }
}