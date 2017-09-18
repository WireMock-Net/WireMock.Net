using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using Xunit;
using WireMock.ResponseBuilders;

namespace WireMock.Net.Tests
{
    //[TestFixture]
    public class ResponseTests
    {
        private const string clientIP = "::1";

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_UrlPathVerb()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", clientIP, body, bodyAsString, Encoding.UTF8);

            var response = Response.Create()
                .WithBody("test {{request.url}} {{request.path}} {{request.method}}")
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test http://localhost/foo /foo post");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Query()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo?a=1&a=2&b=5"), "POST", clientIP, body, bodyAsString, Encoding.UTF8);

            var response = Response.Create()
                .WithBody("test keya={{request.query.a}} idx={{request.query.a.[0]}} idx={{request.query.a.[1]}} keyb={{request.query.b}}")
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test keya=1 idx=1 idx=2 keyb=5");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Headers()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", clientIP, body, bodyAsString, Encoding.UTF8, new Dictionary<string, string> { { "Content-Type", "text/plain" } });

            var response = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}").WithBody("test").WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test");
            Check.That(responseMessage.Headers).Contains(new KeyValuePair<string, string>("x", "text/plain"));
        }

        [Fact]
        public async Task Response_ProvideResponse_Encoding_Body()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", clientIP, body, bodyAsString, Encoding.UTF8);

            var response = Response.Create().WithBody("test", Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test");
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_Encoding_JsonBody()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", clientIP, body, bodyAsString, Encoding.UTF8);

            var response = Response.Create().WithBodyAsJson(new { value = "test" }, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("{\"value\":\"test\"}");
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }
    }
}