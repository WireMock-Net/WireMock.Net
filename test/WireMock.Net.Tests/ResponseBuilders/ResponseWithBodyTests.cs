using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using System;
using System.Text;
using System.Threading.Tasks;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithBodyTests
    {
        private readonly Mock<IWireMockServerSettings> _settingsMock = new Mock<IWireMockServerSettings>();
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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

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
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).IsEqualTo("path: /test");
            Check.That(responseMessage.BodyData.BodyAsBytes).IsNull();
            Check.That(responseMessage.BodyData.BodyAsJson).IsNull();
            Check.That(responseMessage.BodyData.Encoding.CodePage).Equals(Encoding.UTF8.CodePage);
            Check.That(responseMessage.StatusCode).IsEqualTo(500);
            Check.That(responseMessage.Headers["H1"].ToString()).IsEqualTo("X1");
            Check.That(responseMessage.Headers["H2"].ToString()).IsEqualTo("X2");
        }

        [Fact]
        public async Task Response_ProvideResponse_WithJsonBodyAndTransform_Func()
        {
            // Assign
            const int request1Id = 1;
            const int request2Id = 2;

            var request1 = new RequestMessage(new UrlDetails($"http://localhost/test?id={request1Id}"), "GET", ClientIp);
            var request2 = new RequestMessage(new UrlDetails($"http://localhost/test?id={request2Id}"), "GET", ClientIp);

            var response = Response.Create()
                .WithStatusCode(200)
                .WithBodyAsJson(JObject.Parse("{ \"id\": \"{{request.query.id}}\" }"))
                .WithTransformer();

            // Act
            var response1Message = await response.ProvideResponseAsync(request1, _settingsMock.Object);
            var response2Message = await response.ProvideResponseAsync(request2, _settingsMock.Object);

            // Assert
            Check.That(((JToken)response1Message.BodyData.BodyAsJson).SelectToken("id")?.Value<int>()).IsEqualTo(request1Id);
            Check.That(response1Message.BodyData.BodyAsBytes).IsNull();
            Check.That(response1Message.BodyData.BodyAsString).IsNull();
            Check.That(response1Message.StatusCode).IsEqualTo(200);

            Check.That(((JToken)response2Message.BodyData.BodyAsJson).SelectToken("id")?.Value<int>()).IsEqualTo(request2Id);
            Check.That(response2Message.BodyData.BodyAsBytes).IsNull();
            Check.That(response2Message.BodyData.BodyAsString).IsNull();
            Check.That(response2Message.StatusCode).IsEqualTo(200);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBodyAsFile()
        {
            var fileContents = "testFileContents" + Guid.NewGuid();
            var bodyDataAsFile = new BodyData { BodyAsFile = fileContents };

            var request1 = new RequestMessage(new UrlDetails("http://localhost/__admin/files/filename.txt"), "PUT", ClientIp, bodyDataAsFile);

            var response = Response.Create().WithStatusCode(200).WithBody(fileContents);

            var provideResponseAsync = await response.ProvideResponseAsync(request1, _settingsMock.Object);

            Check.That(provideResponseAsync.StatusCode).IsEqualTo(200);
            Check.That(provideResponseAsync.BodyData.BodyAsString).Contains(fileContents);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithResponseAsFile()
        {
            var fileContents = "testFileContents" + Guid.NewGuid();
            var bodyDataAsFile = new BodyData { BodyAsFile = fileContents };

            var request1 = new RequestMessage(new UrlDetails("http://localhost/__admin/files/filename.txt"), "GET", ClientIp, bodyDataAsFile);

            var response = Response.Create().WithStatusCode(200).WithBody(fileContents);

            var provideResponseAsync = await response.ProvideResponseAsync(request1, _settingsMock.Object);

            Check.That(provideResponseAsync.StatusCode).IsEqualTo(200);
            Check.That(provideResponseAsync.BodyData.BodyAsString).Contains(fileContents);
        }

        [Fact]
        public async Task Response_ProvideResponse_WithResponseDeleted()
        {
            var fileContents = "testFileContents" + Guid.NewGuid();
            var bodyDataAsFile = new BodyData { BodyAsFile = fileContents };

            var request1 = new RequestMessage(new UrlDetails("http://localhost/__admin/files/filename.txt"), "DELETE", ClientIp, bodyDataAsFile);

            var response = Response.Create().WithStatusCode(200).WithBody("File deleted.");

            var provideResponseAsync = await response.ProvideResponseAsync(request1, _settingsMock.Object);

            Check.That(provideResponseAsync.StatusCode).IsEqualTo(200);
            Check.That(provideResponseAsync.BodyData.BodyAsString).Contains("File deleted.");
        }
    }
}
