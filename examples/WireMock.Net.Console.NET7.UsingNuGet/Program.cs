// Copyright © WireMock.Net

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using MimeKit;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.NET7.UsingNuGet;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new WireMockConsoleLogger(),
        });

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/test")
            )
            .RespondWith(Response.Create()
                .WithBody(requestMessage => requestMessage.BodyAsMimeMessage != null ?
                    "BodyAsMimeMessage is present" :
                    "BodyAsMimeMessage is not present")
        );

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/some/endpoint")
            )
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
            );

        var httpClient = server.CreateClient();
        var content = new StringContent("abc", Encoding.UTF8, "application/json");

        await TestAsync(httpClient, content);

        await TestNoMultiPartAsync(httpClient, content);

        await TestMultiPartAsync(server);
    }

    private static async Task TestNoMultiPartAsync(HttpClient httpClient, StringContent content)
    {
        var response = await httpClient.PostAsync("/test", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Be("BodyAsMimeMessage is not present");
    }

    private static async Task TestAsync(HttpClient httpClient, StringContent content)
    {
        var response = await httpClient.PostAsync("some/endpoint", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadAsStringAsync()).Should().BeEmpty();
    }

    private static async Task TestMultiPartAsync(WireMockServer server)
    {
        var textPlainContent = "This is some plain text";
        var textPlainContentType = "text/plain";

        var textJson = "{ \"Key\" : \"Value\" }";
        var textJsonContentType = "text/json";

        var imagePngBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAIAAAACAgMAAAAP2OW3AAAADFBMVEX/tID/vpH/pWX/sHidUyjlAAAADElEQVR4XmMQYNgAAADkAMHebX3mAAAAAElFTkSuQmCC");

        server
            .Given(
                Request.Create()
                    .UsingPost()
                    .WithPath("/multipart")
            )
            .RespondWith(Response.Create()
                .WithBody(requestMessage => requestMessage.BodyAsMimeMessage is MimeMessage mm ?
                    "BodyAsMimeMessage is present: " + ((MimePart)mm.BodyParts.Last()).FileName :
                    "BodyAsMimeMessage is not present")
            );

        // Act
        var formDataContent = new MultipartFormDataContent
        {
            { new StringContent(textPlainContent, Encoding.UTF8, textPlainContentType), "text" },
            { new StringContent(textJson, Encoding.UTF8, textJsonContentType), "json" }
        };

        var fileContent = new ByteArrayContent(imagePngBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        formDataContent.Add(fileContent, "somefile", "image.png");

        var client = server.CreateClient();

        var response = await client.PostAsync("/multipart", formDataContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Be("BodyAsMimeMessage is present: image.png");
    }
}