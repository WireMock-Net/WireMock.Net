using System;
using System.Threading.Tasks;
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
    public class ResponseWithHandlebarsHumanizerTests
    {
        private const string ClientIp = "::1";

        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        public ResponseWithHandlebarsHumanizerTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Humanizer()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    DateTime = string.Format("{{{{[Humanizer.Humanize] \"{0}\" }}}}", DateTime.UtcNow.AddHours(-30).ToString("O"))
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            Check.That(j["DateTime"].Value<string>()).IsEqualTo("yesterday");
        }
    }
}