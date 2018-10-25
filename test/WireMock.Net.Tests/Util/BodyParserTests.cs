using System.IO;
using System.Text;
using System.Threading.Tasks;
using NFluent;
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
            // Assign
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
            // Assign
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
        [InlineData(null, "hello", BodyType.String, BodyType.Bytes)]
        public async Task BodyParser_Parse_ContentTypeIsNull(string contentType, string bodyAsString, BodyType detectedBodyType, BodyType detectedBodyTypeFromContentType)
        {
            // Assign
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
    }
}