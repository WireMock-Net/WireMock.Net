#if PROTOBUF
using System.Collections.Generic;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithProtoBufTests
{
    private const string GrpcServiceMethod = "greet.Greeter.SayHello";
    private const string TestProtoDefinition = @"
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
    public void RequestBuilder_WithGrpcProto_Without_JsonMatcher()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithGrpcProto(TestProtoDefinition, GrpcServiceMethod);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);

        var protoBufMatcher = (ProtoBufMatcher)((RequestMessageProtoBufMatcher)matchers[0]).Matcher;
        protoBufMatcher.ProtoDefinition.Should().Be(TestProtoDefinition);
        protoBufMatcher.GrpcServiceMethod.Should().Be(GrpcServiceMethod);
        protoBufMatcher.JsonMatcher.Should().BeNull();
    }

    [Fact]
    public void RequestBuilder_WithGrpcProto_With_JsonMatcher()
    {
        // Act
        var jsonMatcher = new JsonMatcher(new { name = "stef" });
        var requestBuilder = (Request)Request.Create().WithGrpcProto(TestProtoDefinition, GrpcServiceMethod, jsonMatcher);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);

        var protoBufMatcher = (ProtoBufMatcher)((RequestMessageProtoBufMatcher)matchers[0]).Matcher;
        protoBufMatcher.ProtoDefinition.Should().Be(TestProtoDefinition);
        protoBufMatcher.GrpcServiceMethod.Should().Be(GrpcServiceMethod);
        protoBufMatcher.JsonMatcher.Should().BeOfType<JsonMatcher>();
    }
}
#endif