using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;
#if NET452
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithTransformerTests
    {
        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        private const string ClientIp = "::1";

        public ResponseWithTransformerTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_WithNullBody_ShouldNotThrowException(TransformerType transformerType)
        {
            // Assign
            var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
            var request = new RequestMessage(urlDetails, "GET", ClientIp);

            var response = Response.Create().WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            responseMessage.BodyData.Should().BeNull();
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_UrlPathVerb(TransformerType transformerType)
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "whatever",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POSt", ClientIp, body);

            var response = Response.Create()
                .WithBody("test {{request.Url}} {{request.Path}} {{request.Method}}")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test http://localhost/foo /foo POSt");
        }

        [Theory]
        [InlineData(TransformerType.Handlebars, "Get")]
        [InlineData(TransformerType.Handlebars, "Post")]
        [InlineData(TransformerType.Scriban, "Get")]
        [InlineData(TransformerType.Scriban, "Post")]
        [InlineData(TransformerType.ScribanDotLiquid, "Get")]
        [InlineData(TransformerType.ScribanDotLiquid, "Post")]
        public async Task Response_ProvideResponse_Transformer_UrlPath(TransformerType transformerType, string httpMethod)
        {
            // Assign
            var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
            var request = new RequestMessage(urlDetails, httpMethod, ClientIp);

            var response = Response.Create()
                .WithBody("url={{request.Url}} absoluteurl={{request.AbsoluteUrl}} path={{request.Path}} absolutepath={{request.AbsolutePath}}")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("url=http://localhost/a/b absoluteurl=http://localhost/wiremock/a/b path=/a/b absolutepath=/wiremock/a/b");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_PathSegments()
        {
            // Assign
            var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
            var request = new RequestMessage(urlDetails, "POST", ClientIp);

            var response = Response.Create()
                .WithBody("{{request.PathSegments.[0]}} {{request.AbsolutePathSegments.[0]}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("a wiremock");
        }

        [Theory(Skip = "Invalid token `OpenBracket`")]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Scriban_PathSegments(TransformerType transformerType)
        {
            // Assign
            var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
            var request = new RequestMessage(urlDetails, "POST", ClientIp);

            var response = Response.Create()
                .WithBody("{{request.PathSegments.[0]}} {{request.AbsolutePathSegments.[0]}}")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("a wiremock");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Query()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?a=1&a=2&b=5"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("test keya={{request.query.a}} idx={{request.query.a.[0]}} idx={{request.query.a.[1]}} keyb={{request.query.b}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test keya=1,2 idx=1 idx=2 keyb=5");
        }

        [Theory(Skip = "Invalid token `OpenBracket`")]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Scriban_Query(TransformerType transformerType)
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?a=1&a=2&b=5"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("test keya={{request.query.a}} idx={{request.query.a.[0]}} idx={{request.query.a.[1]}} keyb={{request.query.b}}")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test keya=1 idx=1 idx=2 keyb=5");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_StatusCode()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?a=400"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithStatusCode("{{request.query.a}}")
                .WithBody("test")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test");
            Check.That(responseMessage.StatusCode).Equals("400");
        }

        [Theory(Skip = "WireMockList is not supported by Scriban")]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Scriban_StatusCode(TransformerType transformerType)
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?a=400"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithStatusCode("{{request.Query.a}}")
                .WithBody("test")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test");
            Check.That(responseMessage.StatusCode).Equals("400");
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_StatusCodeIsNull(TransformerType transformerType)
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?a=400"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("test")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test");
            Check.That(responseMessage.StatusCode).Equals(null);
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Header()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body, new Dictionary<string, string[]> { { "Content-Type", new[] { "text/plain" } } });

            var response = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}").WithBody("test").WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test");
            Check.That(responseMessage.Headers).ContainsKey("x");
            Check.That(responseMessage.Headers["x"]).ContainsExactly("text/plain");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Headers()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body, new Dictionary<string, string[]> { { "Content-Type", new[] { "text/plain" } } });

            var response = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}", "{{request.url}}").WithBody("test").WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test");
            Check.That(responseMessage.Headers).ContainsKey("x");
            Check.That(responseMessage.Headers["x"]).Contains("text/plain");
            Check.That(responseMessage.Headers["x"]).Contains("http://localhost/foo");
        }

        [Theory(Skip = "WireMockList is not supported by Scriban")]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Scriban_Headers(TransformerType transformerType)
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body, new Dictionary<string, string[]> { { "Content-Type", new[] { "text/plain" } } });

            var response = Response.Create().WithHeader("x", "{{request.Headers[\"Content-Type\"]}}", "{{request.Url}}").WithBody("test").WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test");
            Check.That(responseMessage.Headers).ContainsKey("x");
            Check.That(responseMessage.Headers["x"]).Contains("text/plain");
            Check.That(responseMessage.Headers["x"]).Contains("http://localhost/foo");
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_Origin_Port_Protocol_Host(TransformerType transformerType)
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "abc",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("test {{request.Origin}} {{request.Port}} {{request.Protocol}} {{request.Host}}")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test http://localhost:1234 1234 http localhost");
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_WithBodyAsJson_ResultAsObject(TransformerType transformerType)
        {
            // Assign
            string jsonString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                DetectedBodyType = BodyType.Json,
                Encoding = Encoding.UTF8
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

            var response = Response.Create()
                .WithBodyAsJson(new { x = "test {{request.Path}}" })
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(JsonConvert.SerializeObject(responseMessage.BodyData.BodyAsJson)).Equals("{\"x\":\"test /foo_object\"}");
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        //[InlineData(TransformerType.Scriban)] Scriban cannot access dynamic Json Objects
        //[InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_WithBodyAsJson_ResultAsArray(TransformerType transformerType)
        {
            // Assign
            string jsonString = "{ \"a\": \"test 1\", \"b\": \"test 2\" }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                DetectedBodyType = BodyType.Json,
                Encoding = Encoding.UTF8
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo_array"), "POST", ClientIp, bodyData);

            var response = Response.Create()
                .WithBodyAsJson(new[] { "first", "{{request.path}}", "{{request.bodyAsJson.a}}", "{{request.bodyAsJson.b}}", "last" })
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(JsonConvert.SerializeObject(responseMessage.BodyData.BodyAsJson)).Equals("[\"first\",\"/foo_array\",\"test 1\",\"test 2\",\"last\"]");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_WithBodyAsFile()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?MyUniqueNumber=1"), "GET", ClientIp);

            var response = Response.Create()
                .WithTransformer()
                .WithBodyFromFile(@"c:\\{{request.query.MyUniqueNumber}}\\test.xml");

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsFile).Equals(@"c:\1\test.xml");
        }

        [Theory(Skip = @"Does not work in Scriban --> c:\\[""1""]\\test.xml")]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Scriban_WithBodyAsFile(TransformerType transformerType)
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?MyUniqueNumber=1"), "GET", ClientIp);

            var response = Response.Create()
                .WithTransformer(transformerType)
                .WithBodyFromFile(@"c:\\{{request.query.MyUniqueNumber}}\\test.xml");

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsFile).Equals(@"c:\1\test.xml");
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        //[InlineData(TransformerType.Scriban)] ["c:\\["1"]\\test.xml"]
        //[InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_WithBodyAsFile_And_TransformContentFromBodyAsFile(TransformerType transformerType)
        {
            // Assign
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("<xml MyUniqueNumber=\"{{request.query.MyUniqueNumber}}\"></xml>");

            _settings.FileSystemHandler = filesystemHandlerMock.Object;

            var request = new RequestMessage(new UrlDetails("http://localhost/foo?MyUniqueNumber=1"), "GET", ClientIp);

            var response = Response.Create()
                .WithTransformer(transformerType, true)
                .WithBodyFromFile(@"c:\\{{request.query.MyUniqueNumber}}\\test.xml");

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsFile).Equals(@"c:\1\test.xml");
            Check.That(responseMessage.BodyData.DetectedBodyType).Equals(BodyType.String);
            Check.That(responseMessage.BodyData.BodyAsString).Equals("<xml MyUniqueNumber=\"1\"></xml>");
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_WithBodyAsJson_ResultAsNormalString(TransformerType transformerType)
        {
            // Assign
            string jsonString = "{ \"name\": \"WireMock\" }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                DetectedBodyType = BodyType.Json,
                Encoding = Encoding.UTF8
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

            var response = Response.Create()
                .WithBodyAsJson("test")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(JsonConvert.SerializeObject(responseMessage.BodyData.BodyAsJson)).Equals("\"test\"");
        }

        [Fact(Skip = "todo...")]
        public async Task Response_ProvideResponse_Handlebars_WithBodyAsJson_ResultAsTemplatedString()
        {
            // Assign
            string jsonString = "{ \"name\": \"WireMock\" }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                DetectedBodyType = BodyType.Json,
                Encoding = Encoding.UTF8
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

            var response = Response.Create()
                .WithBodyAsJson("{{{request.BodyAsJson}}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(JsonConvert.SerializeObject(responseMessage.BodyData.BodyAsJson)).Equals("{\"name\":\"WireMock\"}");
        }

        [Theory(Skip = "{{{ }}} Does not work in Scriban")]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Scriban_WithBodyAsJson_ResultAsTemplatedString(TransformerType transformerType)
        {
            // Assign
            string jsonString = "{ \"name\": \"WireMock\" }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                DetectedBodyType = BodyType.Json,
                Encoding = Encoding.UTF8
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

            var response = Response.Create()
                .WithBodyAsJson("{{{request.BodyAsJson}}}")
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(JsonConvert.SerializeObject(responseMessage.BodyData.BodyAsJson)).Equals("{\"name\":\"WireMock\"}");
        }

        [Theory]
        [InlineData(TransformerType.Handlebars)]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public async Task Response_ProvideResponse_Transformer_WithBodyAsString_KeepsEncoding(TransformerType transformerType)
        {
            // Assign
            const string text = "my-text";
            Encoding enc = Encoding.Unicode;
            var bodyData = new BodyData
            {
                BodyAsString = text,
                DetectedBodyType = BodyType.String,
                Encoding = enc
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

            var response = Response.Create()
                .WithBody("{{request.Body}}", BodyDestinationFormat.SameAsSource, enc)
                .WithTransformer(transformerType);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            responseMessage.BodyData.BodyAsString.Should().Be(text);
            responseMessage.BodyData.Encoding.Should().Be(enc);
        }
    }
}