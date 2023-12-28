#if PROTOBUF
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Greet;
using Grpc.Net.Client;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

// ReSharper disable once CheckNamespace
namespace WireMock.Net.Tests;
public partial class WireMockServerTests
{
    [Fact]
    public async Task WireMockServer_WithBodyAsProtoBuf_UsingGrpcGeneratedClient()
    {
        // Arrange
        var jsonMatcher = new JsonMatcher(new { name = "stef" });

        using var server = WireMockServer.Start(settings =>
        {
            settings.Urls = new[] { "grpc://localhost:9093" };
        });

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloRequest", jsonMatcher)
            )
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/grpc")
                .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloReply",
                    new
                    {
                        message = "hello stef"
                    }
                )
                .WithTrailingHeader("grpc-status", "0")
            );

        // Act
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var channel = GrpcChannel.ForAddress("http://localhost:9093", new GrpcChannelOptions
        {
            HttpHandler = handler,
            Credentials = Grpc.Core.ChannelCredentials.Insecure
        });

        var client = new Greeter.GreeterClient(channel);

        var reply = await client.SayHelloAsync(new HelloRequest { Name = "stef" });

        // Assert
        reply.Message.Should().Be("hello stef");

        server.Stop();
    }
}
#endif