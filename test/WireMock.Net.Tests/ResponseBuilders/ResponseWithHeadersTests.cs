using System.Collections.Generic;
using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHeadersTests
    {
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();
        private const string ClientIp = "::1";

        [Theory]
        [InlineData("Content-Length", "1024")]
        [InlineData("Transfer-Encoding", "identity")]
        [InlineData("Location", "http://test")]
        public async Task Response_ProvideResponse_WithHeader_SingleValue(string headerName, string headerValue)
        {
            // Assign
            var requestMock = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp);
            IResponseBuilder builder = Response.Create().WithHeader(headerName, headerValue);

            // Act
            var response = await builder.ProvideResponseAsync(requestMock, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.Headers[headerName].ToString()).Equals(headerValue);
        }

        [Theory]
        [InlineData("Test", new[] { "one" })]
        [InlineData("Test", new[] { "a", "b" })]
        public async Task Response_ProvideResponse_WithHeader_MultipleValues(string headerName, string[] headerValues)
        {
            // Assign
            var requestMock = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp);
            IResponseBuilder builder = Response.Create().WithHeader(headerName, headerValues);

            // Act
            var response = await builder.ProvideResponseAsync(requestMock, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.Headers[headerName].ToArray()).Equals(headerValues);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithHeaders_SingleValue()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);
            var headers = new Dictionary<string, string> { { "h", "x" } };
            var response = Response.Create().WithHeaders(headers);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(responseMessage.Message.Headers["h"]).ContainsExactly("x");
        }

        [Fact]
        public async Task Response_ProvideResponse_WithHeaders_MultipleValues()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);
            var headers = new Dictionary<string, string[]> { { "h", new[] { "x" } } };
            var responseBuilder = Response.Create().WithHeaders(headers);

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.Headers["h"]).ContainsExactly("x");
        }

        [Fact]
        public async Task Response_ProvideResponse_WithHeaders_WiremockList()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);
            var headers = new Dictionary<string, WireMockList<string>> { { "h", new WireMockList<string>("x") } };
            var builder = Response.Create().WithHeaders(headers);

            // Act
            var response = await builder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.Headers["h"]).ContainsExactly("x");
        }
    }
}