using FluentAssertions;
using System;
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
        public async Task Response_ProvideResponse_WithBodyFromFile_AbsolutePath()
        {
            // Arrange
            var server = WireMockServer.Start();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings", "MyXmlResponse.xml");
            Console.WriteLine(path);
            Console.WriteLine($"Checking if file {path} exists: {File.Exists(path)}");
            foreach (var file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings")))
            {
                Console.WriteLine(file); // full path
                Console.WriteLine(System.IO.Path.GetFileName(file)); // file name
            }

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
            string path = @"subdirectory/MyXmlResponse.xml";

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
        public async Task Response_ProvideResponse_WithBodyFromFile_InAdminMappingFolder()
        {
            // Arrange
            var server = WireMockServer.Start();
            string path = @"MyXmlResponse.xml";

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
    }
}