using NFluent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util
{
    public class BodyParserTests
    {
        [Theory]
        [InlineData("application/json", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
        [InlineData("application/json; charset=utf-8", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
        [InlineData("application/json; odata.metadata=minimal", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
        [InlineData("application/vnd.api+json", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
        [InlineData("application/vnd.test+json", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
        public async Task BodyParser_Parse_ContentTypeJson(string contentType, string bodyAsJson, BodyType detectedBodyType, BodyType detectedBodyTypeFromContentType)
        {
            // Arrange
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsJson));

            // Act
            var body = await BodyParser.Parse(memoryStream, contentType);

            // Assert
            Check.That(body.BodyAsBytes).IsNotNull();
            Check.That(body.BodyAsJson).IsNotNull();
            Check.That(body.BodyAsString).Equals(bodyAsJson);
            Check.That(body.DetectedBodyType).IsEqualTo(detectedBodyType);
            Check.That(body.DetectedBodyTypeFromContentType).IsEqualTo(detectedBodyTypeFromContentType);
        }

        [Theory]
        [InlineData("application/xml", "<xml>hello</xml>", BodyType.String, BodyType.String)]
        [InlineData("something", "hello", BodyType.String, BodyType.Bytes)]
        public async Task BodyParser_Parse_ContentTypeString(string contentType, string bodyAsString, BodyType detectedBodyType, BodyType detectedBodyTypeFromContentType)
        {
            // Arrange
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsString));

            // Act
            var body = await BodyParser.Parse(memoryStream, contentType);

            // Assert
            Check.That(body.BodyAsBytes).IsNotNull();
            Check.That(body.BodyAsJson).IsNull();
            Check.That(body.BodyAsString).Equals(bodyAsString);
            Check.That(body.DetectedBodyType).IsEqualTo(detectedBodyType);
            Check.That(body.DetectedBodyTypeFromContentType).IsEqualTo(detectedBodyTypeFromContentType);
        }

        [Fact]
        public async Task BodyParser_Parse_WithUTF8EncodingAndContentTypeMultipart_DetectedBodyTypeEqualsString()
        {
            // Arrange
            string contentType = "multipart/form-data";
            string body = @"

-----------------------------9051914041544843365972754266
Content-Disposition: form-data; name=""text""

text default
-----------------------------9051914041544843365972754266
Content-Disposition: form-data; name=""file1""; filename=""a.txt""
Content-Type: text/plain

Content of a txt

-----------------------------9051914041544843365972754266
Content-Disposition: form-data; name=""file2""; filename=""a.html""
Content-Type: text/html

<!DOCTYPE html><title>Content of a.html.</title>

-----------------------------9051914041544843365972754266--";

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(body));

            // Act
            var result = await BodyParser.Parse(memoryStream, contentType);

            // Assert
            Check.That(result.DetectedBodyType).IsEqualTo(BodyType.String);
            Check.That(result.DetectedBodyTypeFromContentType).IsEqualTo(BodyType.MultiPart);
            Check.That(result.BodyAsBytes).IsNotNull();
            Check.That(result.BodyAsJson).IsNull();
            Check.That(result.BodyAsString).IsNotNull();
        }

        [Fact]
        public async Task BodyParser_Parse_WithUTF16EncodingAndContentTypeMultipart_DetectedBodyTypeEqualsString()
        {
            // Arrange
            string contentType = "multipart/form-data";
            string body = char.ConvertFromUtf32(0x1D161); //U+1D161 = MUSICAL SYMBOL SIXTEENTH NOTE

            var memoryStream = new MemoryStream(Encoding.UTF32.GetBytes(body));

            // Act
            var result = await BodyParser.Parse(memoryStream, contentType);

            // Assert
            Check.That(result.DetectedBodyType).IsEqualTo(BodyType.Bytes);
            Check.That(result.DetectedBodyTypeFromContentType).IsEqualTo(BodyType.MultiPart);
            Check.That(result.BodyAsBytes).IsNotNull();
            Check.That(result.BodyAsJson).IsNull();
            Check.That(result.BodyAsString).IsNull();
        }

        [Theory]
        [InlineData(null, "hello", BodyType.String, BodyType.Bytes)]
        public async Task BodyParser_Parse_ContentTypeIsNull(string contentType, string bodyAsString, BodyType detectedBodyType, BodyType detectedBodyTypeFromContentType)
        {
            // Arrange
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsString));

            // Act
            var body = await BodyParser.Parse(memoryStream, contentType);

            // Assert
            Check.That(body.BodyAsBytes).IsNotNull();
            Check.That(body.BodyAsJson).IsNull();
            Check.That(body.BodyAsString).Equals(bodyAsString);
            Check.That(body.DetectedBodyType).IsEqualTo(detectedBodyType);
            Check.That(body.DetectedBodyTypeFromContentType).IsEqualTo(detectedBodyTypeFromContentType);
        }

        [Theory]
        [InlineData("HEAD", false)]
        [InlineData("GET", false)]
        [InlineData("PUT", true)]
        [InlineData("POST", true)]
        [InlineData("DELETE", false)]
        [InlineData("TRACE", false)]
        [InlineData("OPTIONS", true)]
        [InlineData("CONNECT", false)]
        [InlineData("PATCH", true)]
        public void BodyParser_ShouldParseBody_ExpectedResultForKnownMethods(string method, bool resultShouldBe)
        {
            Check.That(BodyParser.ShouldParseBody(method)).Equals(resultShouldBe);
        }

        [Theory]
        [InlineData("REPORT")]
        [InlineData("SOME-UNKNOWN-METHOD")]
        public void BodyParser_ShouldParseBody_DefaultIsTrueForUnknownMethods(string method)
        {
            Check.That(BodyParser.ShouldParseBody(method)).IsTrue();
        }
    }
}