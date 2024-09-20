// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NFluent;
using WireMock.Http;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Http;

public class HttpRequestMessageHelperTests
{
    private const string ClientIp = "::1";

    [Fact]
    public void HttpRequestMessageHelper_Create()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "x", new[] { "value-1" } } };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, null, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(message.Headers.GetValues("x")).ContainsExactly("value-1");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Bytes()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsBytes = Encoding.UTF8.GetBytes("hi"),
            DetectedBodyType = BodyType.Bytes
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp, body);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content.ReadAsByteArrayAsync().ConfigureAwait(false)).ContainsExactly(Encoding.UTF8.GetBytes("hi"));
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_TextPlain()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = "0123", // or 83 in decimal
            BodyAsJson = 83,
            DetectedBodyType = BodyType.Json,
            DetectedBodyTypeFromContentType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp, body);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsStringAsync().ConfigureAwait(false)).Equals("0123");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Json()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new { x = 42 },
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp, body);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsStringAsync().ConfigureAwait(false)).Equals("{\"x\":42}");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Json_With_ContentType_ApplicationJson()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/json" } } };
        var body = new BodyData
        {
            BodyAsJson = new { x = 42 },
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content.ReadAsStringAsync().ConfigureAwait(false)).Equals("{\"x\":42}");
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/json");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Json_With_ContentType_ApplicationJson_UTF8()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/json; charset=utf-8" } } };
        var body = new BodyData
        {
            BodyAsJson = new { x = 42 },
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content.ReadAsStringAsync().ConfigureAwait(false)).Equals("{\"x\":42}");
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/json; charset=utf-8");
    }

    [Fact]
    public void HttpRequestMessageHelper_Create_String_With_ContentType_ApplicationXml()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/xml" } } };
        var body = new BodyData
        {
            BodyAsString = "<xml>hello</xml>",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/xml");
    }

    [Fact]
    public void HttpRequestMessageHelper_Create_String_With_ContentType_ApplicationXml_UTF8()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/xml; charset=UTF-8" } } };
        var body = new BodyData
        {
            BodyAsString = "<xml>hello</xml>",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/xml; charset=UTF-8");
    }

    [Fact]
    public void HttpRequestMessageHelper_Create_String_With_ContentType_ApplicationXml_ASCII()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/xml; charset=Ascii" } } };
        var body = new BodyData
        {
            BodyAsString = "<xml>hello</xml>",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/xml; charset=Ascii");
    }

    [Theory]
    [InlineData("HEAD", true)]
    [InlineData("GET", false)]
    [InlineData("PUT", false)]
    [InlineData("POST", false)]
    [InlineData("DELETE", false)]
    [InlineData("TRACE", false)]
    [InlineData("OPTIONS", false)]
    [InlineData("CONNECT", false)]
    [InlineData("PATCH", false)]
    public void HttpRequestMessageHelper_Create_ContentLengthAllowedForMethod(string method, bool resultShouldBe)
    {
        // Arrange
        var key = "Content-Length";
        var value = 1234;
        var headers = new Dictionary<string, string[]> { { key, new[] { "1234" } } };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), method, ClientIp, null, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        message.Content?.Headers.ContentLength.Should().Be(resultShouldBe ? value : null);
    }
}