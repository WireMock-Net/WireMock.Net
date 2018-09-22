using System;
using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilderTests
{
    public class ResponseCreateTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_Create()
        {
            // Assign
            var responseMessage = new ResponseMessage { StatusCode = 500 };
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

            var response = Response.Create(() => responseMessage);

            // Act
            var providedResponse = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(providedResponse).Equals(responseMessage);
        }
    }
}