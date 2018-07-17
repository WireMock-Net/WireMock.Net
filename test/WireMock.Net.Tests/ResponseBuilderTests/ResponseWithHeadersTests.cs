using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilderTests
{
    public class ResponseWithHeadersTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponse_WithHeaders_SingleValue()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);
            var headers = new Dictionary<string, string> { { "h", "x" } };
            var response = Response.Create().WithHeaders(headers);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Headers["h"]).ContainsExactly("x");
        }

        [Fact]
        public async Task Response_ProvideResponse_WithHeaders_MultipleValues()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);
            var headers = new Dictionary<string, string[]> { { "h", new[] { "x" } } };
            var response = Response.Create().WithHeaders(headers);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Headers["h"]).ContainsExactly("x");
        }

        [Fact]
        public async Task Response_ProvideResponse_WithHeaders_WiremockList()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);
            var headers = new Dictionary<string, WireMockList<string>> { { "h", new WireMockList<string>("x") } };
            var response = Response.Create().WithHeaders(headers);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Headers["h"]).ContainsExactly("x");
        }
    }
}