using FluentAssertions;
using Moq;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Owin;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests;

public class MappingBuilderTests
{
    private static readonly string MappingGuid = "41372914-1838-4c67-916b-b9aacdd096ce";

    private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock;

    private readonly MappingBuilder _sut;

    public MappingBuilderTests()
    {
        _fileSystemHandlerMock = new Mock<IFileSystemHandler>();

        var settings = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object,
            Logger = Mock.Of<IWireMockLogger>()
        };
        var options = new WireMockMiddlewareOptions();
        var matcherMapper = new MatcherMapper(settings);
        var mappingConverter = new MappingConverter(matcherMapper);
        var mappingToFileSaver = new MappingToFileSaver(settings, mappingConverter);

        _sut = new MappingBuilder(settings, options, mappingConverter, mappingToFileSaver);

        _sut.Given(Request.Create()
            .WithPath("/foo")
            .UsingGet()
        )
        .WithGuid(MappingGuid)
        .RespondWith(Response.Create()
            .WithBody(@"{ msg: ""Hello world!""}")
        );
    }

    [Fact]
    public void GetMappings()
    {
        // Act
        var mappings = _sut.GetMappings();

        // Assert
        mappings.Should().HaveCount(1);
    }

    [Fact]
    public void ToJson()
    {
        // Act
        var json = _sut.ToJson();

        // Assert
        json.Should().NotBeEmpty();
    }

    [Fact]
    public void SaveMappingsToFile_FolderExists_IsFalse()
    {
        // Arrange
        var path = "path";

        // Act
        _sut.SaveMappingsToFile(path);

        // Verify
        _fileSystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Never);
        _fileSystemHandlerMock.Verify(fs => fs.FolderExists(path), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.CreateFolder(path), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.WriteMappingFile(path, It.IsAny<string>()), Times.Once);
        _fileSystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void SaveMappingsToFile_FolderExists_IsTrue()
    {
        // Arrange
        var path = "path";
        _fileSystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);

        // Act
        _sut.SaveMappingsToFile(path);

        // Verify
        _fileSystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Never);
        _fileSystemHandlerMock.Verify(fs => fs.FolderExists(path), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.WriteMappingFile(path, It.IsAny<string>()), Times.Once);
        _fileSystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void SaveMappingsToFolder_FolderIsNull()
    {
        // Arrange
        var mappingFolder = "mapping-folder";
        _fileSystemHandlerMock.Setup(fs => fs.GetMappingFolder()).Returns(mappingFolder);
        _fileSystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);

        // Act
        _sut.SaveMappingsToFolder(null);

        // Verify
        _fileSystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.FolderExists(mappingFolder), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _fileSystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void SaveMappingsToFolder_FolderExists_IsTrue()
    {
        // Arrange
        var path = "path";
        _fileSystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);

        // Act
        _sut.SaveMappingsToFolder(path);

        // Verify
        _fileSystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Never);
        _fileSystemHandlerMock.Verify(fs => fs.FolderExists(path), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _fileSystemHandlerMock.VerifyNoOtherCalls();
    }
}