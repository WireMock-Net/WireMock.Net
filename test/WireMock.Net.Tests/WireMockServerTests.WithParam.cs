using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    [Theory]
    [InlineData("SELECT id, value FROM table WHERE id = 1")]
    [InlineData("select id, name, value from table where id in (1, 2, 3, 4, 5)")]
    [InlineData("1,2,3")]
    public async Task WireMockServer_WithParam_QueryParameterMultipleValueSupport_NoComma_Should_Ignore_Comma(string queryValue)
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            QueryParameterMultipleValueSupport = QueryParameterMultipleValueSupport.NoComma
        };
        var server = WireMockServer.Start(settings);
        server.Given(
            Request.Create()
                .UsingGet()
                .WithPath("/foo")
                .WithParam("query", queryValue)
            )
            .RespondWith(
                Response.Create().WithStatusCode(200)
            );

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/foo?query={queryValue}");
        var response = await server.CreateClient().GetAsync(requestUri).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithParam_MultiValueComma()
    {
        // Arrange
        var queryValue = "1,2,3";
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .UsingGet()
                .WithPath("/foo")
                .WithParam("query", "1", "2", "3")
            )
            .RespondWith(
                Response.Create().WithStatusCode(200)
            );

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/foo?query={queryValue}");
        var response = await server.CreateClient().GetAsync(requestUri).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithParam_RejectOnMatch_OnNonMatchingParam_ShouldReturnMappingOk()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .WithPath("/v1/person/workers")
                .WithParam("delta_from", MatchBehaviour.RejectOnMatch)
                .UsingGet()
            )
            .RespondWith(
                Response.Create()
            );

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/v1/person/workers?showsourcesystem=true&count=700&page=1&sections=personal%2Corganizations%2Cemployment");
        var response = await server.CreateClient().GetAsync(requestUri).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithParam_AcceptOnMatch_OnNonMatchingParam_ShouldReturnMappingOk()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .WithPath("/v1/person/workers")
                .WithParam("delta_from")
                .UsingGet()
            )
            .RespondWith(
                Response.Create()
            );

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/v1/person/workers?showsourcesystem=true&count=700&page=1&sections=personal%2Corganizations%2Cemployment");
        var response = await server.CreateClient().GetAsync(requestUri).ConfigureAwait(false);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        server.Stop();
    }
}