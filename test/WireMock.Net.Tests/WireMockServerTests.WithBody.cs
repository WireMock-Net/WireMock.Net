#if !NET452
//using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
//using System.Net.Http.Json;
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
}
#endif