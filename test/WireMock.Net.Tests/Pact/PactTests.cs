using System.IO;
using System.Net;
using WireMock.Matchers;
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

    [Fact]
    public void SavePact_Multiple_Requests()
    {
        var server = WireMockServer.Start();
        server
            .WithConsumer("Something API Consumer Multiple")
            .WithProvider("Something API")

            .Given(Request.Create()
                .UsingPost()
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

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/add")
                .WithHeader("Accept", "application/json")
                .WithBody(new JsonMatcher("{ \"Id\" : \"1\", \"FirstName\" : \"Totally\" }"))
            )
            .WithTitle("A Post request to add the something")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.RedirectMethod)
                    .WithBodyAsJson(new
                    {
                        Id = "1",
                        FirstName = "Totally"
                    })
            );

        server.SavePact(Path.Combine("../../../", "Pact", "files"), "pact-multiple.json");
    }
}