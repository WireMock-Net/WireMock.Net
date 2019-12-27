using NFluent;
using System.Collections.Generic;
using System.Text;
using WireMock.Http;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Http
{
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
        public async void HttpRequestMessageHelper_Create_Bytes()
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
            Check.That(await message.Content.ReadAsByteArrayAsync()).ContainsExactly(Encoding.UTF8.GetBytes("hi"));
        }

        [Fact]
        public async void HttpRequestMessageHelper_Create_Json()
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
            Check.That(await message.Content.ReadAsStringAsync()).Equals("{\"x\":42}");
        }

        [Fact]
        public async void HttpRequestMessageHelper_Create_Json_With_ContentType_ApplicationJson()
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
            Check.That(await message.Content.ReadAsStringAsync()).Equals("{\"x\":42}");
            Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/json");
        }

        [Fact]
        public async void HttpRequestMessageHelper_Create_Json_With_ContentType_ApplicationJson_UTF8()
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
            Check.That(await message.Content.ReadAsStringAsync()).Equals("{\"x\":42}");
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
    }
}