using System;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using WireMock.ResponseBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilderTests
{
    public class ResponseWithBodyTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponse_WithBody_Bytes_Encoding_Destination_String()
        {
            // given
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body);

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
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body);

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
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body);

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
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body);

            object x = new { value = "test" };
            var response = Response.Create().WithBodyAsJson(x, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.BodyAsJson).IsNotNull();
            Check.That(responseMessage.BodyAsJson).Equals(x);
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_String_SameAsSource_Encoding()
        {
            // Assign
            var request = new RequestMessage(new Uri("http://localhost"), "GET", ClientIp);

            var response = Response.Create().WithBody("r", BodyDestinationFormat.SameAsSource, Encoding.ASCII);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.BodyAsBytes).IsNull();
            Check.That(responseMessage.BodyAsJson).IsNull();
            Check.That(responseMessage.Body).Equals("r");
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_String_Bytes_Encoding()
        {
            // Assign
            var request = new RequestMessage(new Uri("http://localhost"), "GET", ClientIp);

            var response = Response.Create().WithBody("r", BodyDestinationFormat.Bytes, Encoding.ASCII);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Body).IsNull();
            Check.That(responseMessage.BodyAsJson).IsNull();
            Check.That(responseMessage.BodyAsBytes).IsNotNull();
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_String_Json_Encoding()
        {
            // Assign
            var request = new RequestMessage(new Uri("http://localhost"), "GET", ClientIp);

            var response = Response.Create().WithBody("{ \"value\": 42 }", BodyDestinationFormat.Json, Encoding.ASCII);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Body).IsNull();
            Check.That(responseMessage.BodyAsBytes).IsNull();
            Check.That(((dynamic)responseMessage.BodyAsJson).value).Equals(42);
            Check.That(responseMessage.BodyEncoding).Equals(Encoding.ASCII);
        }
    }
}