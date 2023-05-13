using System.IO;
using log4net.Config;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.ConsoleApplication;

static class Program
{
    static void Main(params string[] args)
    {
        XmlConfigurator.Configure(new FileInfo("log4net.config"));

        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("todos")
                .UsingGet()
            )
            .RespondWith(Response.Create()
                .WithBody("test")
            );

        server
            .Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl("some-url").And
            .WithHeader("header-name", "header-value");

        server.Stop();

        // MainApp.Run();
    }
}