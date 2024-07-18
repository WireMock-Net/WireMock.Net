// Copyright © WireMock.Net

using System.Threading.Tasks;
using Moq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsHelpersTests
{
    private const string ClientIp = "::1";

    private readonly WireMockServerSettings _settings = new();

    public ResponseWithHandlebarsHelpersTests()
    {
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_HandlebarsHelpers_String_Uppercase()
    {
        // Assign
        var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithBody("{{String.Uppercase request.body}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(new Mock<IMapping>().Object, request, _settings).ConfigureAwait(false);

        // assert
        Check.That(response.Message.BodyData.BodyAsString).Equals("ABC");
    }
}