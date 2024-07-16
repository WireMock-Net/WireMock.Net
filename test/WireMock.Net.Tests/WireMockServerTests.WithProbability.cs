// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    [Fact]
    public async Task WireMockServer_WithProbability()
    {
        // Arrange
        var server = WireMockServer.Start();
        server
            .Given(Request.Create().UsingGet().WithPath("/foo"))
            .WithProbability(0.5)
            .RespondWith(Response.Create().WithStatusCode(200));

        server
            .Given(Request.Create().UsingGet().WithPath("/foo"))
            .RespondWith(Response.Create().WithStatusCode(500));

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/foo");
        var response = await server.CreateClient().GetAsync(requestUri).ConfigureAwait(false);

        // Assert
        Assert.True(new[] { HttpStatusCode.OK, HttpStatusCode.InternalServerError }.Contains(response.StatusCode));

        server.Stop();
    }
}