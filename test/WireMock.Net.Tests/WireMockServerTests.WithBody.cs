// Copyright © WireMock.Net

#if !NET452
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    public class DummyClass
    {
        public string? Hi { get; set; }
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsJsonAsync_And_MultipleJmesPathMatchers_ShouldMatch()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .WithPath("/a")
                .WithBody(
                    new IMatcher[]
                    {
                        new JmesPathMatcher("requestId == '1'"),
                        new JmesPathMatcher("value == 'A'")
                    },
                    MatchOperator.And
                )
                .UsingPost()
            )
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));

        server.Given(
                Request.Create()
                    .WithPath("/a")
                    .WithBody(
                        new IMatcher[]
                        {
                            new JmesPathMatcher("requestId == '2'"),
                            new JmesPathMatcher("value == 'A'")
                        },
                        MatchOperator.And
                    )
                    .UsingPost()
            )
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Moved));

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/a");

        var json = new { requestId = "1", value = "A" };
        var response = await server.CreateClient().PostAsJsonAsync(requestUri, json).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsJsonAsync_And_MultipleJmesPathMatchers_ShouldMatch_BestMatching()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
                Request.Create()
                    .WithPath("/a")
                    .WithBody(
                        new IMatcher[]
                        {
                            new JmesPathMatcher("extra == 'X'"),
                            new JmesPathMatcher("requestId == '1'"),
                            new JmesPathMatcher("value == 'A'")
                        },
                        MatchOperator.And
                    )
                    .UsingPost()
            )
            .AtPriority(1) // Higher priority
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));

        server.Given(
                Request.Create()
                    .WithPath("/a")
                    .WithBody(
                        new IMatcher[]
                        {
                            new JmesPathMatcher("requestId == '1'"),
                            new JmesPathMatcher("value == 'A'")
                        },
                        MatchOperator.And
                    )
                    .UsingPost()
            )
            .AtPriority(2)
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Moved));

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/a");

        var json = new { extra = "X", requestId = "1", value = "A" };
        var response = await server.CreateClient().PostAsJsonAsync(requestUri, json).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsJsonAsync_And_WildcardMatcher_ShouldMatch()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
                Request.Create().UsingPost().WithPath("/foo").WithBody(new WildcardMatcher("*Hello*"))
            )
            .RespondWith(
                Response.Create().WithStatusCode(200)
            );

        var jsonObject = new DummyClass
        {
            Hi = "Hello World!"
        };

        // Act
        var response = await new HttpClient().PostAsJsonAsync("http://localhost:" + server.Ports[0] + "/foo", jsonObject).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsync_And_WildcardMatcher_ShouldMatch()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
                Request.Create().UsingPost().WithPath("/foo").WithBody(new WildcardMatcher("*Hello*"))
            )
            .RespondWith(
                Response.Create().WithStatusCode(200)
            );

        // Act
        var response = await new HttpClient().PostAsync("http://localhost:" + server.Ports[0] + "/foo", new StringContent("{ Hi = \"Hello World\" }")).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsFormUrlEncoded_Using_PostAsync_And_WithFunc()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .UsingPost()
                .WithPath("/foo")
                .WithBody(values => values != null && values["key1"] == "value1")
            )
            .RespondWith(
                Response.Create()
            );

        // Act
        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("key1", "value1") });
        var response = await new HttpClient()
            .PostAsync($"{server.Url}/foo", content)
            .ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsFormUrlEncoded_Using_PostAsync_And_WithExactMatcher()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
                Request.Create()
                    .UsingPost()
                    .WithPath("/foo")
                    .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                    .WithBody(new ExactMatcher("name=John+Doe&email=johndoe%40example.com")
                )
            )
            .RespondWith(
                Response.Create()
            );

        // Act
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        });
        var response = await new HttpClient()
            .PostAsync($"{server.Url}/foo", content)
            .ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsFormUrlEncoded_Using_PostAsync_And_WithFormUrlEncodedMatcher()
    {
        // Arrange
        var matcher = new FormUrlEncodedMatcher(["email=johndoe@example.com", "name=John Doe"]);
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .UsingPost()
                .WithPath("/foo")
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithBody(matcher)
            )
            .RespondWith(
                Response.Create()
            );

        server.Given(
            Request.Create()
                .UsingPost()
                .WithPath("/bar")
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithBody(matcher)
            )
            .RespondWith(
                Response.Create()
            );

        // Act 1
        var contentOrdered = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        });
        var responseOrdered = await new HttpClient()
            .PostAsync($"{server.Url}/foo", contentOrdered)
            .ConfigureAwait(false);

        // Assert 1
        responseOrdered.StatusCode.Should().Be(HttpStatusCode.OK);


        // Act 2
        var contentUnordered = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", "johndoe@example.com"),
            new KeyValuePair<string, string>("name", "John Doe"),
        });
        var responseUnordered = await new HttpClient()
            .PostAsync($"{server.Url}/bar", contentUnordered)
            .ConfigureAwait(false);

        // Assert 2
        responseUnordered.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }
}
#endif