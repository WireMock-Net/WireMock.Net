using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseCreateTests
    {
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        [Fact]
        public async Task Response_Create_Func()
        {
            // Assign
            var responseMessage = new ResponseMessage { StatusCode = 500 };
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", "::1");

            var response = Response.Create(() => responseMessage);

            // Act
            var providedResponse = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(providedResponse).Equals(responseMessage);
        }
    }
}