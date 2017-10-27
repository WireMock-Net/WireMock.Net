using System;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using WireMock.ResponseBuilders;
using Xunit;

namespace WireMock.Net.Tests
{
    public partial class ResponseTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponse_WithBody_Bytes_Encoding_Destination_String()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            var response = Response.Create().WithBody(new byte[] { 48, 49 }, BodyDestinationFormat.String, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("01");
            Check.That(responseMessage.BodyAsBytes).IsNull();
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_Bytes_Encoding_Destination_Bytes()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            var response = Response.Create().WithBody(new byte[] { 48, 49 }, BodyDestinationFormat.SameAsSource, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.BodyAsBytes).ContainsExactly(new byte[] { 48, 49 });
            Check.That(responseMessage.Body).IsNull();
            Check.That(responseMessage.BodyEncoding).IsNull();
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_String_Encoding()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            var response = Response.Create().WithBody("test", null, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test");
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_Object_Encoding()
        {
            // given
            string bodyAsString = "abc";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            var response = Response.Create().WithBodyAsJson(new { value = "test" }, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("{\"value\":\"test\"}");
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }
    }
}