#if PROTOBUF
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    private const string ProtoDefinition = @"
syntax = ""proto3"";

package greet;

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
";
    [Fact]
    public async Task WireMockServer_WithGrpcProto()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");
        var jsonMatcher = new JsonMatcher(new { name = "stef" });

        using var server = WireMockServer.Start();

        server
            .Given(
                Request.Create()
                    .UsingPost()
                    .WithPath("/grpc/greet-Greeter-SayHello")
                    .WithGrpcProto(ProtoDefinition, "greet.HelloRequest", jsonMatcher)
            )
            .RespondWith(Response.Create()
                .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloReply", new
                {
                    message = "hello"
                })
            );

        // Act
        var protoBuf = new ByteArrayContent(bytes);
        protoBuf.Headers.ContentType = new MediaTypeHeaderValue("application/grpc-web");

        var client = server.CreateClient();
        var response = await client.PostAsync("/grpc/greet-Greeter-SayHello", protoBuf);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        Convert.ToBase64String(responseBytes).Should().Be("CgVoZWxsbw==");

        server.Stop();
    }
}
#endif