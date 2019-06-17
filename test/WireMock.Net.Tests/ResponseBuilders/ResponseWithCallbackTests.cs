using Moq;
using NFluent;
using System.Threading.Tasks;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithCallbackTests
    {
        private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock = new Mock<IFileSystemHandler>();

        [Fact]
        public async Task Response_WithCallback()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var response = Response.Create().WithCallback(req => new ResponseMessage { BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = req.Path + "Bar" }, StatusCode = 302 });

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _fileSystemHandlerMock.Object);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).IsEqualTo("/fooBar");
            Check.That(responseMessage.StatusCode).IsEqualTo(302);
        }
    }
}