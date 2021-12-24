using System.Threading.Tasks;
using HandlebarsDotNet;
using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsFileTests
    {
        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();
        private const string ClientIp = "::1";

        public ResponseWithHandlebarsFileTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_File()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    Data = "{{File \"x.json\"}}"
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            Check.That(j["Data"].Value<string>()).Equals("abc");

            // Verify
            _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString("x.json"), Times.Once);
            _filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_File_Replace()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234?id=x"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    Data = "{{File \"{{request.query.id}}.json\"}}"
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            Check.That(j["Data"].Value<string>()).Equals("abc");

            // Verify
            _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString("x.json"), Times.Once);
            _filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Response_ProvideResponseAsync_Handlebars_File_WithMissingArgument_Throws_HandlebarsException()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    Data = "{{File}}"
                })
                .WithTransformer();

            // Act
            Check.ThatAsyncCode(() => responseBuilder.ProvideResponseAsync(request, _settings)).Throws<HandlebarsException>();

            // Verify
            _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString(It.IsAny<string>()), Times.Never);
            _filesystemHandlerMock.VerifyNoOtherCalls();
        }
    }
}