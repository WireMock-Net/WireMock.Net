// Copyright © WireMock.Net

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    private const string RequestXml =
        """
        <xml>
        	<Contact FirstName = "Stef" />
        </xml>
        """;

    private readonly string _responseFilePath = Path.Combine(Environment.CurrentDirectory, "__admin", "mappings", "responseWithTransformer.xml");

    [Fact]
    public async Task WireMockServer_WithTransformer_WithBodyShouldWorkWithTransformer()
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
            .WhenRequest(req => req
                .WithPath("/withbody")
                .UsingPost())
            .ThenRespondWith(rsp => rsp
                .WithSuccess()
                .WithBody(File.ReadAllText(_responseFilePath))
                .WithTransformer());

        // Act
        var response = await GetResponseAsync(server, "/withbody");

        // Assert
        response.Should().Contain("Hello, Stef!");
    }

    [Fact]
    public async Task WireMockServer_WithTransformer_WithBodyFromFileShouldWorkWithTransformer()
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
            .WhenRequest(req => req
                .WithPath("/withbodyfromfile")
                .UsingPost())
            .ThenRespondWith(rsp => rsp
                .WithSuccess()
                .WithBodyFromFile(_responseFilePath)
                .WithTransformer(transformContentFromBodyAsFile: true));

        // Act
        var response = await GetResponseAsync(server, "/withbodyfromfile");

        // Assert
        response.Should().Contain("Hello, Stef!");
    }

    private static async Task<string> GetResponseAsync(WireMockServer server, string relativePath)
    {
        using HttpClient httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(server.Urls[0]);

        using var requestContent = new StringContent(RequestXml);
        using var responseMsg = await httpClient.PostAsync(relativePath, requestContent);
        return await responseMsg.Content.ReadAsStringAsync();
    }
}