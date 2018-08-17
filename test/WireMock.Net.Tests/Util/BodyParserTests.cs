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
        [Fact]
        public async Task BodyParser_Parse_ApplicationJson()
        {
            // Assign
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("{ \"x\": 1 }"));

            // Act
            var body = await BodyParser.Parse(memoryStream, "application/json");

            // Assert
            Check.That(body.BodyAsBytes).IsNull();
            Check.That(body.BodyAsJson).IsNotNull();
            Check.That(body.BodyAsString).Equals("{ \"x\": 1 }");
        }

        [Fact] // http://jsonapi.org/
        public async Task BodyParser_Parse_ApplicationJsonApi()
        {
            // Assign
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("{ \"x\": 1 }"));

            // Act
            var body = await BodyParser.Parse(memoryStream, "application/vnd.api+json");

            // Assert
            Check.That(body.BodyAsBytes).IsNull();
            Check.That(body.BodyAsJson).IsNotNull();
            Check.That(body.BodyAsString).Equals("{ \"x\": 1 }");
        }

        [Fact]
        public async Task BodyParser_Parse_ApplicationXml()
        {
            // Assign
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("<xml>hello</xml>"));
            
            // Act
            var body = await BodyParser.Parse(memoryStream, "application/xml; charset=UTF-8");

            // Assert
            Check.That(body.BodyAsBytes).IsNull();
            Check.That(body.BodyAsJson).IsNull();
            Check.That(body.BodyAsString).Equals("<xml>hello</xml>");
        }
    }
}