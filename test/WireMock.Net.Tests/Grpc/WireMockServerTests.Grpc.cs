// Copyright © WireMock.Net

#if PROTOBUF
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

    [Theory]
    [InlineData("CgRzdGVm")]
    [InlineData("AAAAAAYKBHN0ZWY=")]
    public async Task WireMockServer_WithBodyAsProtoBuf(string data)
    {
        // Arrange
        var bytes = Convert.FromBase64String(data);
        var jsonMatcher = new JsonMatcher(new { name = "stef" });

        using var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/grpc/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloRequest", jsonMatcher)
            )
            .RespondWith(Response.Create()
                    .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloReply",
                        new
                        {
                            message = "hello"
                        }
                    )
            .WithTrailingHeader("grpc-status", "0")
            );

        // Act
        var protoBuf = new ByteArrayContent(bytes);
        protoBuf.Headers.ContentType = new MediaTypeHeaderValue("application/grpc-web");

        var client = server.CreateClient();
        var response = await client.PostAsync("/grpc/greet.Greeter/SayHello", protoBuf);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        Convert.ToBase64String(responseBytes).Should().Be("AAAAAAcKBWhlbGxv");

        server.Stop();
    }

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
                .WithTrailingHeader("grpc-status", "0")
                .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloReply",
                    new
                    {
                        message = "hello stef {{request.method}}"
                    }
                )
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

    [Fact]
    public async Task WireMockServer_WithBodyAsProtoBuf_MappingProtoDefinition_UsingGrpcGeneratedClient()
    {
        // Arrange
        using var server = WireMockServer.Start(useHttp2: true);

        var jsonMatcher = new JsonMatcher(new { name = "stef" });

        server
            .Given(Request.Create()
                .UsingPost()
                .WithHttpVersion("2")
                .WithPath("/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf("greet.HelloRequest", jsonMatcher)
            )
            .WithProtoDefinition(ProtoDefinition)
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/grpc")
                .WithTrailingHeader("grpc-status", "0")
                .WithBodyAsProtoBuf("greet.HelloReply",
                    new
                    {
                        message = "hello {{request.BodyAsJson.name}} {{request.method}}"
                    }
                )
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

    [Fact]
    public async Task WireMockServer_WithBodyAsProtoBuf_ServerProtoDefinition_UsingGrpcGeneratedClient()
    {
        // Arrange
        var id = $"test-{Guid.NewGuid()}";

        using var server = WireMockServer.Start(useHttp2: true);

        var jsonMatcher = new JsonMatcher(new { name = "stef" });

        server
            .AddProtoDefinition(id, ProtoDefinition)
            .Given(Request.Create()
                .UsingPost()
                .WithHttpVersion("2")
                .WithPath("/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf("greet.HelloRequest", jsonMatcher)
            )
            .WithProtoDefinition(id)
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/grpc")
                .WithTrailingHeader("grpc-status", "0")
                .WithBodyAsProtoBuf("greet.HelloReply",
                    new
                    {
                        message = "hello {{request.BodyAsJson.name}} {{request.method}}"
                    }
                )
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