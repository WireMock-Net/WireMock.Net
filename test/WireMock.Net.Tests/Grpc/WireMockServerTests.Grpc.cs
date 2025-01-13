// Copyright © WireMock.Net

#if PROTOBUF
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Greet;
using Grpc.Net.Client;
using WireMock.Constants;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Util;
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

    private const string ProtoDefinitionWithWellKnownTypes = @"
syntax = ""proto3"";

package communication.api.v1;

import ""google/protobuf/empty.proto"";
import ""google/protobuf/timestamp.proto"";
import ""google/protobuf/duration.proto"";

service Greeter {
    rpc SayNothing (google.protobuf.Empty) returns (google.protobuf.Empty);
}

message MyMessageTimestamp {
    google.protobuf.Timestamp ts = 1;
}

message MyMessageDuration {
    google.protobuf.Duration du = 1;
}
";

    private const string ProtoDefinitionMain = @"
syntax = ""proto3"";

package greet;

import ""other.proto"";
import ""google/protobuf/empty.proto"";

service Greeter {
  rpc Nothing (google.protobuf.Empty) returns (google.protobuf.Empty);
  
  rpc SayHello (HelloRequest) returns (HelloReply);

  rpc SayOther (Other) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}

message Person {
  string name = 1;
  int32 id = 2;
  string email = 3;
}
";

    private const string ProtoDefinitionOther = @"// other.proto
syntax = ""proto3"";

package greet;

message Other {
  string name = 1;
}
";

    [Fact]
    public async Task WireMockServer_WithBodyAsProtoBuf_JsonPartialWildcardMatcher()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");
        var jsonMatcher = new JsonPartialWildcardMatcher(new { name = "*" });

        using var server = WireMockServer.Start();

        server
            .WhenRequest(r => r
                .UsingPost()
                .WithPath("/grpc/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloRequest", jsonMatcher)
            )
            .ThenRespondWith(r => r
                .WithTrailingHeader("grpc-status", "0")
            );

        // Act
        var protoBuf = new ByteArrayContent(bytes);
        protoBuf.Headers.ContentType = new MediaTypeHeaderValue("application/grpc-web");

        var client = server.CreateClient();
        var response = await client.PostAsync("/grpc/greet.Greeter/SayHello", protoBuf);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

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
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsProtoBuf_WithWellKnownTypes()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");

        using var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/grpc/Greeter/SayNothing")
                .WithBody(new NotNullOrEmptyMatcher())
            )
            .RespondWith(Response.Create()
                .WithBodyAsProtoBuf(ProtoDefinitionWithWellKnownTypes, "google.protobuf.Empty",
                    new { }
                )
                .WithTrailingHeader("grpc-status", "0")
                .WithTransformer()
            );

        // Act
        var protoBuf = new ByteArrayContent(bytes);
        protoBuf.Headers.ContentType = new MediaTypeHeaderValue("application/grpc-web");

        var client = server.CreateClient();
        var response = await client.PostAsync("/grpc/Greeter/SayNothing", protoBuf);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        Convert.ToBase64String(responseBytes).Should().Be("");
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsProtoBuf_ServerProtoDefinition_WithWellKnownTypes()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");

        using var server = WireMockServer.Start();

        var id = $"proto-{Guid.NewGuid()}";

        server
            .AddProtoDefinition(id, ProtoDefinitionWithWellKnownTypes)
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/grpc/Greeter/SayNothing")
                .WithBody(new NotNullOrEmptyMatcher())
            )
            .WithProtoDefinition(id)
            .RespondWith(Response.Create()
                .WithBodyAsProtoBuf("google.protobuf.Empty",
                    new { }
                )
                .WithTrailingHeader("grpc-status", "0")
                .WithTransformer()
            );

        // Act
        var protoBuf = new ByteArrayContent(bytes);
        protoBuf.Headers.ContentType = new MediaTypeHeaderValue("application/grpc-web");

        var client = server.CreateClient();
        var response = await client.PostAsync("/grpc/Greeter/SayNothing", protoBuf);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        Convert.ToBase64String(responseBytes).Should().Be("");
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsProtoBuf_MultipleFiles()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");
        var jsonMatcher = new JsonMatcher(new { name = "stef" });

        using var server = WireMockServer.Start();

        var protoFiles = new[] { ProtoDefinitionMain, ProtoDefinitionOther };

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/grpc/greet.Greeter/SayOther")
                .WithBodyAsProtoBuf(protoFiles, "greet.Other", jsonMatcher)
            )
            .RespondWith(Response.Create()
                .WithBodyAsProtoBuf(protoFiles, "greet.HelloReply",
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
        var response = await client.PostAsync("/grpc/greet.Greeter/SayOther", protoBuf);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        Convert.ToBase64String(responseBytes).Should().Be("AAAAAAcKBWhlbGxv");
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
        Then_ReplyMessage_Should_BeCorrect(reply);
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
        var reply = await When_GrpcClient_Calls_SayHelloAsync(server.Url!);

        // Assert
        Then_ReplyMessage_Should_BeCorrect(reply);
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsProtoBuf_ServerProtoDefinitionFromJson_UsingGrpcGeneratedClient()
    {
        var server = Given_When_ServerStartedUsingHttp2();
        Given_ProtoDefinition_IsAddedOnServerLevel(server);
        await Given_When_ProtoBufMappingIsAddedViaAdminInterfaceAsync(server);

        var reply = await When_GrpcClient_Calls_SayHelloAsync(server.Urls[1]);

        Then_ReplyMessage_Should_BeCorrect(reply);
    }

    private static WireMockServer Given_When_ServerStartedUsingHttp2()
    {
        var ports = PortUtils.FindFreeTcpPorts(2);

        var settings = new WireMockServerSettings
        {
            Urls = [$"http://*:{ports[0]}/", $"grpc://*:{ports[1]}/"],
            StartAdminInterface = true
        };
        return WireMockServer.Start(settings);
    }

    private static void Given_ProtoDefinition_IsAddedOnServerLevel(WireMockServer server)
    {
        server.AddProtoDefinition("my-greeter", ReadProtoFile("greet.proto"));
    }

    private static async Task Given_When_ProtoBufMappingIsAddedViaAdminInterfaceAsync(WireMockServer server)
    {
        var mappingsJson = ReadMappingFile("protobuf-mapping-3.json");

        using var httpClient = server.CreateClient();

        var result = await httpClient.PostAsync("/__admin/mappings", new StringContent(mappingsJson, Encoding.UTF8, WireMockConstants.ContentTypeJson));
        result.EnsureSuccessStatusCode();
    }

    private static async Task<HelloReply> When_GrpcClient_Calls_SayHelloAsync(string address)
    {
        var channel = GrpcChannel.ForAddress(address);

        var client = new Greeter.GreeterClient(channel);

        return await client.SayHelloAsync(new HelloRequest { Name = "stef" });
    }

    private static void Then_ReplyMessage_Should_BeCorrect(HelloReply reply)
    {
        reply.Message.Should().Be("hello stef POST");
    }

    private static string ReadMappingFile(string filename)
    {
        return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings", filename));
    }

    private static string ReadProtoFile(string filename)
    {
        return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Grpc", filename));
    }
}
#endif