#if PROTOBUF
using System;
using FluentAssertions;
using ProtoBuf;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class ProtoBufMatcherTests
{
    private const string MessageType = "greet.HelloRequest";
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
    public void ProtoBufMatcher_For_ValidProtoBuf_And_ValidMethod_NoJsonMatchers_IsMatch()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");

        // Act
        var matcher = new ProtoBufMatcher(ProtoDefinition, MessageType);
        var result = matcher.IsMatch(bytes);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void ProtoBufMatcher_For_ValidProtoBuf_And_ValidMethod_Using_JsonMatcher_IsMatch()
    {
        // Arrange
        var jsonMatcher = new JsonMatcher(new { name = "stef" });
        var bytes = Convert.FromBase64String("CgRzdGVm");

        // Act
        var matcher = new ProtoBufMatcher(ProtoDefinition, MessageType, jsonMatcher: jsonMatcher);
        var result = matcher.IsMatch(bytes);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void ProtoBufMatcher_For_InvalidProtoBuf_IsNoMatch()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };

        // Act
        var matcher = new ProtoBufMatcher(ProtoDefinition, MessageType);
        var result = matcher.IsMatch(bytes);

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().BeOfType<ProtoException>();
    }

    [Fact]
    public void ProtoBufMatcher_For_InvalidMethod_IsNoMatch()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");

        // Act
        var matcher = new ProtoBufMatcher(ProtoDefinition, "greet.Greeter.X");
        var result = matcher.IsMatch(bytes);

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().BeOfType<ArgumentException>();
    }
}
#endif