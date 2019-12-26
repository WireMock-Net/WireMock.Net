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
        private readonly Mock<IFluentMockServerSettings> _settingsMock = new Mock<IFluentMockServerSettings>();
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
            var response = Response.Create();
            switch (statusCode)
            {
                case string statusCodeAsString:
                    response = response.WithStatusCode(statusCodeAsString);
                    break;

                case int statusCodeAInteger:
                    response = response.WithStatusCode(statusCodeAInteger);
                    break;

                case HttpStatusCode statusCodeAsEnum:
                    response = response.WithStatusCode(statusCodeAsEnum);
                    break;
            }

            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            responseMessage.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
