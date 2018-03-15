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