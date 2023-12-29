#if PROTOBUF
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
    public async Task WireMockServer_WithBodyAsProtoBuf_InlineProtoDefinition_UsingGrpcGeneratedClient()
    {
        // Arrange
        using var server = WireMockServer.Start(useHttp2: true);

        var jsonMatcher = new JsonMatcher(new { name = "stef" });

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
                        message = "hello stef {{request.method}}"
                    }
                )
                .WithTrailingHeader("grpc-status", "0")
                .WithTransformer()
            );

        // Act
        var channel = GrpcChannel.ForAddress(server.Url!);

        var client = new Greeter.GreeterClient(channel);

        var reply = await client.SayHelloAsync(new HelloRequest { Name = "stef" });

        // Assert
        reply.Message.Should().Be("hello stef POST");

        server.Stop();
    }
}
#endif