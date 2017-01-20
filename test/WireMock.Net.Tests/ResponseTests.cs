using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using WireMock.ResponseBuilders;

namespace WireMock.Net.Tests
{
    [TestFixture]
    public class ResponseTests
    {
        [Test]
        public async Task Response_ProvideResponse_Handlebars_body()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString);

            var response = Response.Create().WithBody("test {{request.url}} {{request.path}} {{request.verb}}").WithTransformer();
            
            // act
            var responseMessage = await response.ProvideResponse(request);

            // then
            Check.That(responseMessage.Body).Equals("test http://localhost/foo /foo post");
        }

        [Test]
        public async Task Response_ProvideResponse_Handlebars_headers()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString, new Dictionary<string, string> { { "Content-Type", "text/plain" } });

            var response = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}").WithBody("test").WithTransformer();

            // act
            var responseMessage = await response.ProvideResponse(request);

            // then
            Check.That(responseMessage.Body).Equals("test");
            Check.That(responseMessage.Headers).Contains(new KeyValuePair<string,string>("x", "text/plain"));
        }
    }
}