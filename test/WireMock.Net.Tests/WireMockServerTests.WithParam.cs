using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    [Fact]
    public async Task WireMockServer_WithParam_QueryParameterMultipleValueSupport_NoComma_Should_Ignore_Comma()
    {
        // Arrange
        var queryValue = "SELECT id, value FROM table WHERE id = 1";
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
}