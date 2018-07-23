using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using Xunit;

namespace WireMock.Net.Tests
{
    public class ResponseTests
    {
        private const string ClientIp = "::1";

        [Theory]
        [InlineData("Content-Length", "1024")]
        [InlineData("Transfer-Encoding", "identity")]
        [InlineData("Location", "http://test")]
        public async void Response_Create_WithHeader(string headerName, string headerValue)
        {
            // Assign
            var requestMock = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp);
            IResponseBuilder builder = Response.Create().WithHeader(headerName, headerValue);

            // Act
            var response = await builder.ProvideResponseAsync(requestMock);

            // Assert
            Check.That(response.Headers[headerName].ToString()).Equals(headerValue);
        }
    }
}