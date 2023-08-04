using System.Net;
using System.Text;
using FluentAssertions;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.NET7.UsingNuGet;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new WireMockConsoleLogger(),
        });

        server.Given(Request.Create().UsingPost().WithPath("/some/endpoint"))
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Created));

        var httpClient = new HttpClient { BaseAddress = new Uri(server.Url!) };
        var requestUri = new Uri(httpClient.BaseAddress!, "some/endpoint");
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

        // Act
        var actual = await httpClient.PostAsync(requestUri, content);

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}