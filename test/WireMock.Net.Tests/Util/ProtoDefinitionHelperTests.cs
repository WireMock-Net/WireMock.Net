using System.IO;
using FluentAssertions;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class ProtoDefinitionHelperTests
{
    [Fact]
    public void FromDirectory_ShouldReturnModifiedProtoFiles()
    {
        // Arrange
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Grpc", "Test");
        var expectedFilename = $"SubFolder{Path.DirectorySeparatorChar}request.proto";
        var expectedComment = $"// {expectedFilename}";

        // Act
        var protoDefinitions = ProtoDefinitionHelper.FromDirectory(directory);

        // Assert
        protoDefinitions.Should().HaveCount(2);
        protoDefinitions[0].Should().StartWith("// greet.proto");
        protoDefinitions[1].Should().StartWith(expectedComment);

#if PROTOBUF
        // Arrange
        var resolver = new WireMockProtoFileResolver(protoDefinitions);

        // Act + Assert
        resolver.Exists(expectedFilename).Should().BeTrue();
        resolver.Exists("x").Should().BeFalse();

        // Act + Assert
        var text = resolver.OpenText(expectedFilename).ReadToEnd();
        text.Should().StartWith(expectedComment);
        System.Action action = () => resolver.OpenText("x");
        action.Should().Throw<FileNotFoundException>();
#endif
    }
}