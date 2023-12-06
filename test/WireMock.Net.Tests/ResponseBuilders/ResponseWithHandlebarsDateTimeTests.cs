using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsDateTimeTests
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithHandlebarsDateTimeTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_WithBodyAsJson_ProvideResponseAsync_Handlebars_DateTime()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                DateTimeYear = "{{ DateTime.UtcNow \"yyyy\" }}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        var j = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        j["DateTimeYear"]!.Value<string>().Should().Be(DateTime.Now.Year.ToString());
    }

    [Fact]
    public async Task Response_WithBody_ProvideResponseAsync_Handlebars_DateTime()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("{\nDateTimeYear = \"{{ DateTime.UtcNow \"yyyy\" }}\"")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Contain($"DateTimeYear = \"{DateTime.Now.Year}\"");
    }
}