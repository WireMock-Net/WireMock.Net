using System.IO;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests.Pact;

public class PactTests
{
    [Fact]
    public void SavePact_Get_Request()
    {
        var server = WireMockServer.Start();
        server
            .WithConsumer("Something API Consumer Get")
            .WithProvider("Something API")
            .Given(Request.Create()
                .UsingGet()
                .WithPath("/tester")
                .WithParam("q1", "test")
                .WithParam("q2", "ok")
                .WithHeader("Accept", "application/json")
            )
            .WithTitle("A GET request to retrieve the something")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBodyAsJson(new
                    {
                        Id = "tester",
                        FirstName = "Totally",
                        LastName = "Awesome"
                    })
            );

        server.SavePact(Path.Combine("../../../", "Pact", "files"), "pact-get.json");
    }
}