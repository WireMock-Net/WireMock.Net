using FluentAssertions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithBodyFromFileTests
    {
        [Fact]
        public async Task Response_ProvideResponse_WithBodyFromFile()
        {
            // Arrange
            var server = FluentMockServer.Start();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings", "MyXmlResponse.xml");

            server
                .Given(
                    Request
                        .Create()
                        .UsingGet()
                        .WithPath("/v1/content")
                )
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(path)
                );

            // Act
            var response = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content");

            // Assert
            response.Should().Contain("<hello>world</hello>");
        }
    }
}