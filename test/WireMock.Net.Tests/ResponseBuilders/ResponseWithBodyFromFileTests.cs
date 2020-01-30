using FluentAssertions;
using System.Collections.Generic;
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
            var server = WireMockServer.Start();

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
            var response1 = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content");
            var response2 = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content");
            var response3 = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content");

            // Assert
            response1.Should().Contain("<hello>world</hello>");
            response2.Should().Contain("<hello>world</hello>");
            response3.Should().Contain("<hello>world</hello>");
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBodyFromFile_InSubDirectory()
        {
            // Arrange
            var server = WireMockServer.Start();
            int path = 0;
            List<string> pathList = new List<string>()
            {
                @"subdirectory/MyXmlResponse.xml",
                @"subdirectory\MyXmlResponse.xml",
                @"/subdirectory/MyXmlResponse.xml",
                @"\subdirectory\MyXmlResponse.xml"
            };

            server
                .Given(
                    Request
                        .Create()
                        .UsingGet()
                        .WithPath("/v1/content/path1")
                )
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(pathList[path])
                );

            path++;
            server
                .Given(
                    Request
                        .Create()
                        .UsingGet()
                        .WithPath("/v1/content/path2")
                )
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(pathList[path])
                );

            path++;
            server
                .Given(
                    Request
                        .Create()
                        .UsingGet()
                        .WithPath("/v1/content/path3")
                )
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(pathList[path])
                );

            path++;
            server
                .Given(
                    Request
                        .Create()
                        .UsingGet()
                        .WithPath("/v1/content/path4")
                )
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(pathList[path])
                );

            // Act
            var response1 = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content/path1");
            var response2 = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content/path2");
            var response3 = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content/path3");
            var response4 = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content/path4");

            // Assert
            response1.Should().Contain("<hello>world</hello>");
            response2.Should().Contain("<hello>world</hello>");
            response3.Should().Contain("<hello>world</hello>");
            response4.Should().Contain("<hello>world</hello>");
        }
    }
}