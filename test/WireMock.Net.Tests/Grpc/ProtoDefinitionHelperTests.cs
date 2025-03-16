// Copyright © WireMock.Net

#if PROTOBUF
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Grpc;

public class ProtoDefinitionHelperTests
{
    [Fact]
    public async Task FromDirectory_Greet_ShouldReturnModifiedProtoFiles()
    {
        // Arrange
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Grpc", "Test");
        var expectedFilename = "SubFolder/request.proto";
        var expectedComment = $"// {expectedFilename}";

        // Act
        var protoDefinitionData = await ProtoDefinitionHelper.FromDirectory(directory);
        var protoDefinitions = protoDefinitionData.ToList("greet");

        // Assert
        protoDefinitions.Should().HaveCount(2);
        protoDefinitions[0].Should().StartWith("// greet.proto");
        protoDefinitions[1].Should().StartWith(expectedComment);
        
        // Arrange
        var resolver = new WireMockProtoFileResolver(protoDefinitions);

        // Act + Assert
        resolver.Exists(expectedFilename).Should().BeTrue();
        resolver.Exists("x").Should().BeFalse();

        // Act + Assert
        var text = await resolver.OpenText(expectedFilename).ReadToEndAsync();
        text.Should().StartWith(expectedComment);
        System.Action action = () => resolver.OpenText("x");
        action.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public async Task FromDirectory_OpenTelemetry_ShouldReturnModifiedProtoFiles()
    {
        // Arrange
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Grpc", "ot");

        // Act
        var protoDefinitionData = await ProtoDefinitionHelper.FromDirectory(directory);
        var protoDefinitions = protoDefinitionData.ToList("trace_service");

        // Assert
        protoDefinitions.Should().HaveCount(10);
    }
}
#endif