using System;
using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using System.Threading.Tasks;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Transformers;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsFileFragmentTests
    {
        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private const string ClientIp = "::1";

        public ResponseWithHandlebarsFileFragmentTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_FileFragment()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Data = "{{FileFragment \"x.json\"}}"
                })
                .WithTransformer();

            response.SetPrivateFieldValue("_responseMessageTransformer", new ResponseMessageTransformer(_filesystemHandlerMock.Object));

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Data"].Value<string>()).Equals("abc");

            // Verify
            _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString("x.json"), Times.Once);
            _filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_FileFragment_Replace()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234?id=x"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Data = "{{FileFragment \"{{request.query.id}}.json\"}}"
                })
                .WithTransformer();

            response.SetPrivateFieldValue("_responseMessageTransformer", new ResponseMessageTransformer(_filesystemHandlerMock.Object));

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Data"].Value<string>()).Equals("abc");

            // Verify
            _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString("x.json"), Times.Once);
            _filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Response_ProvideResponseAsync_Handlebars_WithMissingArgument_ThrowsArgumentOutOfRangeException()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Data = "{{FileFragment}}"
                })
                .WithTransformer();

            // response.SetPrivateFieldValue("_responseMessageTransformer", new ResponseMessageTransformer(_filesystemHandlerMock.Object));

            // Act
            Check.ThatAsyncCode(() => response.ProvideResponseAsync(request)).Throws<ArgumentOutOfRangeException>();

            // Verify
            _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString(It.IsAny<string>()), Times.Never);
            _filesystemHandlerMock.VerifyNoOtherCalls();
        }
    }
}