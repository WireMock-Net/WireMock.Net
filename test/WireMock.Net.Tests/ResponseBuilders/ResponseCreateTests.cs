using System.Threading.Tasks;
using Moq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseCreateTests
    {
        private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock = new Mock<IFileSystemHandler>();

        [Fact]
        public async Task Response_Create_Func()
        {
            // Assign
            var responseMessage = new ResponseMessage { StatusCode = 500 };
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", "::1");

            var response = Response.Create(() => responseMessage);

            // Act
            var providedResponse = await response.ProvideResponseAsync(request, _fileSystemHandlerMock.Object);

            // Assert
            Check.That(providedResponse).Equals(responseMessage);
        }
    }
}