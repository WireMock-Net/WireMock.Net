#if PROTOBUF
using System;
using FluentAssertions;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class ProtoBufMatcherTests
{
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
    public void ProtoBufMatcher_For_ValidProtoBuf_And_CorrectMethod_And_CorrectBody_IsMatch()
    {
        // Arrange
        var jsonMatcher = new JsonMatcher(new { Name = "stef" });
        var bytes = Convert.FromBase64String("CgRzdGVm");

        // Act
        var matcher = new ProtoBufMatcher(TestProtoDefinition, "greet.Greeter.SayHello");
        var result = matcher.IsMatch(bytes);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
    }
}
#endif