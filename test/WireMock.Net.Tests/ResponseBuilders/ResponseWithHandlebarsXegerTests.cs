using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using System.Threading.Tasks;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsXegerTests
    {
        private const string ClientIp = "::1";

        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        public ResponseWithHandlebarsXegerTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Xeger1()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Number = "{{Xeger \"[1-9]{1}\\d{3}\"}}",
                    Postcode = "{{Xeger \"[1-9][0-9]{3}[A-Z]{2}\"}}"
                })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Number"].Value<int>()).IsStrictlyGreaterThan(1000).And.IsStrictlyLessThan(9999);
            Check.That(j["Postcode"].Value<string>()).IsNotEmpty();
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Xeger2()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Number = "{{#Xeger \"[1-9]{1}\\d{3}\"}}{{this}}{{/Xeger}}",
                    Postcode = "{{#Xeger \"[1-9][0-9]{3}[A-Z]{2}\"}}{{this}}{{/Xeger}}"
                })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Number"].Value<int>()).IsStrictlyGreaterThan(1000).And.IsStrictlyLessThan(9999);
            Check.That(j["Postcode"].Value<string>()).IsNotEmpty();
        }
    }
}