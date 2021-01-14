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

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsHelpersTests
    {
        private const string ClientIp = "::1";

        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        public ResponseWithHandlebarsHelpersTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_HandlebarsHelpers_String_Uppercase()
        {
            // Assign
            var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{String.Uppercase request.body}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("ABC");
        }
    }
}