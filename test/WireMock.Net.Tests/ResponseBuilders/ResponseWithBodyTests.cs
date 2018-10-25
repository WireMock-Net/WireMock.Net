using System.Text;
using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
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
                DetectedBodyType = BodyType.String,
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

            var response = Response.Create().WithBody(new byte[] { 48, 49 }, BodyDestinationFormat.String, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.BodyData.BodyAsString).Equals("01");
            Check.That(responseMessage.BodyData.BodyAsBytes).IsNull();
            Check.That(responseMessage.BodyData.Encoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_Bytes_Encoding_Destination_Bytes()
        {
            // given
            var body = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

            var response = Response.Create().WithBody(new byte[] { 48, 49 }, BodyDestinationFormat.SameAsSource, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.BodyData.BodyAsBytes).ContainsExactly(new byte[] { 48, 49 });
            Check.That(responseMessage.BodyData.BodyAsString).IsNull();
            Check.That(responseMessage.BodyData.Encoding).IsNull();
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_String_Encoding()
        {
            // given
            var body = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

            var response = Response.Create().WithBody("test", null, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test");
            Check.That(responseMessage.BodyData.Encoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_Object_Encoding()
        {
            // given
            var body = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

            object x = new { value = "test" };
            var response = Response.Create().WithBodyAsJson(x, Encoding.ASCII);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.BodyData.BodyAsJson).Equals(x);
            Check.That(responseMessage.BodyData.Encoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_Object_Indented()
        {
            // given
            var body = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

            object x = new { message = "Hello" };
            var response = Response.Create().WithBodyAsJson(x, true);

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.BodyData.BodyAsJson).Equals(x);
            Check.That(responseMessage.BodyData.BodyAsJsonIndented).IsEqualTo(true);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_String_SameAsSource_Encoding()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

            var response = Response.Create().WithBody("r", BodyDestinationFormat.SameAsSource, Encoding.ASCII);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsBytes).IsNull();
            Check.That(responseMessage.BodyData.BodyAsJson).IsNull();
            Check.That(responseMessage.BodyData.BodyAsString).Equals("r");
            Check.That(responseMessage.BodyData.Encoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_String_Bytes_Encoding()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

            var response = Response.Create().WithBody("r", BodyDestinationFormat.Bytes, Encoding.ASCII);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).IsNull();
            Check.That(responseMessage.BodyData.BodyAsJson).IsNull();
            Check.That(responseMessage.BodyData.BodyAsBytes).IsNotNull();
            Check.That(responseMessage.BodyData.Encoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_String_Json_Encoding()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

            var response = Response.Create().WithBody("{ \"value\": 42 }", BodyDestinationFormat.Json, Encoding.ASCII);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).IsNull();
            Check.That(responseMessage.BodyData.BodyAsBytes).IsNull();
            Check.That(((dynamic)responseMessage.BodyData.BodyAsJson).value).Equals(42);
            Check.That(responseMessage.BodyData.Encoding).Equals(Encoding.ASCII);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBody_Func()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost/test"), "GET", ClientIp);

            var response = Response.Create()
                .WithStatusCode(500)
                .WithHeader("H1", "X1")
                .WithHeader("H2", "X2")
                .WithBody(req => $"path: {req.Path}");

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).IsEqualTo("path: /test");
            Check.That(responseMessage.BodyData.BodyAsBytes).IsNull();
            Check.That(responseMessage.BodyData.BodyAsJson).IsNull();
            Check.That(responseMessage.BodyData.Encoding.CodePage).Equals(Encoding.UTF8.CodePage);
            Check.That(responseMessage.StatusCode).IsEqualTo(500);
            Check.That(responseMessage.Headers["H1"].ToString()).IsEqualTo("X1");
            Check.That(responseMessage.Headers["H2"].ToString()).IsEqualTo("X2");
        }
    }
}