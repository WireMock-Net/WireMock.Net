using Moq;
using NFluent;
using System.Threading.Tasks;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseCreateTests
    {
        private readonly Mock<WireMockServerSettings> _settingsMock = new Mock<WireMockServerSettings>();

        [Fact]
        public async Task Response_Create_Func()
        {
            // Assign
            var responseMessage = new ResponseMessage { StatusCode = 500 };
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", "::1");

            var response = Response.Create(() => responseMessage);

            // Act
            var providedResponse = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            Check.That(providedResponse).Equals(responseMessage);
        }
    }
}