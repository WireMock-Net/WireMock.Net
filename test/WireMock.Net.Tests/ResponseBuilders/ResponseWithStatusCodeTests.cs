using FluentAssertions;
using Moq;
using System.Net;
using System.Threading.Tasks;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithStatusCodeTests
    {
        private readonly Mock<WireMockServerSettings> _settingsMock = new Mock<WireMockServerSettings>();
        private const string ClientIp = "::1";

        [Theory]
        [InlineData("201", "201")]
        [InlineData(201, 201)]
        [InlineData(HttpStatusCode.Created, 201)]
        public async Task Response_ProvideResponse_WithStatusCode(object statusCode, object expectedStatusCode)
        {
            // Arrange
            var request = new RequestMessage(new UrlDetails("http://localhost/fault"), "GET", ClientIp);

            // Act
            var responseBuilder = Response.Create();
            switch (statusCode)
            {
                case string statusCodeAsString:
                    responseBuilder = responseBuilder.WithStatusCode(statusCodeAsString);
                    break;

                case int statusCodeAInteger:
                    responseBuilder = responseBuilder.WithStatusCode(statusCodeAInteger);
                    break;

                case HttpStatusCode statusCodeAsEnum:
                    responseBuilder = responseBuilder.WithStatusCode(statusCodeAsEnum);
                    break;
            }

            var response = await responseBuilder.ProvideResponseAsync(request, _settingsMock.Object).ConfigureAwait(false);

            // Assert
            response.Message.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
