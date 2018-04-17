using System;
using NFluent;
using WireMock.ResponseBuilders;
using Xunit;

namespace WireMock.Net.Tests
{
    public class ResponseTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public async void Response_Create_WithHeader_ContentLength()
        {
            // Assign
            var requestMock = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp);
            IResponseBuilder builder = Response.Create().WithHeader("Content-Length", "1024");

            // Act
            var response = await builder.ProvideResponseAsync(requestMock);

            // Assert
            Check.That(response.Headers["Content-Length"].ToString()).Equals("1024");
        }
    }
}