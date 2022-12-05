using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;
using System.Globalization;
#if NET452
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithTransformerTests
{
    private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
    private readonly WireMockServerSettings _settings = new();
    private const string ClientIp = "::1";

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithTransformerTests()
    {
        _mappingMock = new Mock<IMapping>();

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

        var responseBuilder = Response.Create().WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        response.Message.BodyData.Should().BeNull();
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

        var responseBuilder = Response.Create()
            .WithBody("test {{request.Url}} {{request.Path}} {{request.Method}}")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test http://localhost/foo /foo POSt");
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

        var responseBuilder = Response.Create()
            .WithBody("url={{request.Url}} absoluteurl={{request.AbsoluteUrl}} path={{request.Path}} absolutepath={{request.AbsolutePath}}")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("url=http://localhost/a/b absoluteurl=http://localhost/wiremock/a/b path=/a/b absolutepath=/wiremock/a/b");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_PathSegments()
    {
        // Assign
        var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
        var request = new RequestMessage(urlDetails, "POST", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("{{request.PathSegments.[0]}} {{request.AbsolutePathSegments.[0]}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("a wiremock");
    }

    [Theory(Skip = "Invalid token `OpenBracket`")]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public async Task Response_ProvideResponse_Scriban_PathSegments(TransformerType transformerType)
    {
        // Assign
        var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
        var request = new RequestMessage(urlDetails, "POST", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("{{request.PathSegments.[0]}} {{request.AbsolutePathSegments.[0]}}")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("a wiremock");
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

        var responseBuilder = Response.Create()
            .WithBody("test keya={{request.query.a}} idx={{request.query.a.[0]}} idx={{request.query.a.[1]}} keyb={{request.query.b}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test keya=1,2 idx=1 idx=2 keyb=5");
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

        var responseBuilder = Response.Create()
            .WithBody("test keya={{request.query.a}} idx={{request.query.a.[0]}} idx={{request.query.a.[1]}} keyb={{request.query.b}}")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test keya=1 idx=1 idx=2 keyb=5");
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

        var responseBuilder = Response.Create()
            .WithStatusCode("{{request.query.a}}")
            .WithBody("test")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test");
        Check.That(response.Message.StatusCode).Equals("400");
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

        var responseBuilder = Response.Create()
            .WithStatusCode("{{request.Query.a}}")
            .WithBody("test")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test");
        Check.That(response.Message.StatusCode).Equals("400");
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

        var responseBuilder = Response.Create()
            .WithBody("test")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test");
        Check.That(response.Message.StatusCode).Equals(null);
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

        var responseBuilder = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}").WithBody("test").WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test");
        Check.That(response.Message.Headers).ContainsKey("x");
        Check.That(response.Message.Headers!["x"]).ContainsExactly("text/plain");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_Header_TransformMapping()
    {
        // Assign
        var guid = Guid.NewGuid();
        _mappingMock.SetupGet(m => m.Guid).Returns(guid);

        var body = new BodyData
        {
            BodyAsString = "abc",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        var responseBuilder = Response.Create().WithHeader("x", "{{mapping.Guid}}").WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        response.Message.Headers.Should().NotBeNull();
        Check.That(response.Message.Headers).ContainsKey("x");
        Check.That(response.Message.Headers!["x"]).ContainsExactly(guid.ToString());
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

        var responseBuilder = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}", "{{request.url}}").WithBody("test").WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test");
        Check.That(response.Message.Headers).ContainsKey("x");
        Check.That(response.Message.Headers!["x"]).Contains("text/plain");
        Check.That(response.Message.Headers["x"]).Contains("http://localhost/foo");
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

        var responseBuilder = Response.Create().WithHeader("x", "{{request.Headers[\"Content-Type\"]}}", "{{request.Url}}").WithBody("test").WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test");
        Check.That(response.Message.Headers).ContainsKey("x");
        Check.That(response.Message.Headers!["x"]).Contains("text/plain");
        Check.That(response.Message.Headers["x"]).Contains("http://localhost/foo");
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

        var responseBuilder = Response.Create()
            .WithBody("test {{request.Origin}} {{request.Port}} {{request.Protocol}} {{request.Host}}")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsString).Equals("test http://localhost:1234 1234 http localhost");
    }

    [Theory]
    [InlineData(TransformerType.Handlebars)]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public async Task Response_ProvideResponse_Transformer_WithBodyAsJson_ResultAsObject(TransformerType transformerType)
    {
        // Assign
        string jsonString = "{ \"id\": 42, \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"WireMock\" } ] }";
        var bodyData = new BodyData
        {
            BodyAsJson = JsonConvert.DeserializeObject(jsonString),
            DetectedBodyType = BodyType.Json,
            Encoding = Encoding.UTF8
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { x = "test {{request.Path}}" })
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson)).Equals("{\"x\":\"test /foo_object\"}");
    }

    [Theory]
    [InlineData(TransformerType.Handlebars, "{ \"id\": 42 }", "{\"x\":\"test 42\",\"y\":42}")]
    [InlineData(TransformerType.Scriban, "{ \"id\": 42 }", "{\"x\":\"test 42\",\"y\":42}")]
    [InlineData(TransformerType.ScribanDotLiquid, "{ \"id\": 42 }", "{\"x\":\"test 42\",\"y\":42}")]
    [InlineData(TransformerType.Handlebars, "{ \"id\": true }", "{\"x\":\"test True\",\"y\":true}")]
    [InlineData(TransformerType.Scriban, "{ \"id\": true }", "{\"x\":\"test True\",\"y\":true}")]
    [InlineData(TransformerType.ScribanDotLiquid, "{ \"id\": true }", "{\"x\":\"test True\",\"y\":true}")]
    [InlineData(TransformerType.Handlebars, "{ \"id\": 0.005 }", "{\"x\":\"test 0.005\",\"y\":0.005}")]
    [InlineData(TransformerType.Scriban, "{ \"id\": 0.005 }", "{\"x\":\"test 0.005\",\"y\":0.005}")]
    [InlineData(TransformerType.ScribanDotLiquid, "{ \"id\": 0.005 }", "{\"x\":\"test 0.005\",\"y\":0.005}")]
    public async Task Response_ProvideResponse_Transformer_WithBodyAsJson_KeepType(TransformerType transformerType, string jsonString, string expected)
    {
        // Assign
        var culture = CultureInfo.CreateSpecificCulture("en-US");
        var settings = new WireMockServerSettings
        {
            FileSystemHandler = _filesystemHandlerMock.Object,
            Culture = culture
        };
        var jsonSettings = new JsonSerializerSettings
        {
            Culture = culture
        };
        var bodyData = new BodyData
        {
            BodyAsJson = JsonConvert.DeserializeObject(jsonString, jsonSettings),
            DetectedBodyType = BodyType.Json,
            Encoding = Encoding.UTF8
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { x = "test {{request.BodyAsJson.id}}", y = "{{request.BodyAsJson.id}}" })
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, settings).ConfigureAwait(false);

        // Assert
        JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson).Should().Be(expected);
    }

    [Theory]
    [InlineData(TransformerType.Handlebars)]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public async Task Response_ProvideResponse_Transformer_ResultAsArray(TransformerType transformerType)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new[] { new { x = "test" } })
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson).Should().Be("[{\"x\":\"test\"}]");
    }

    [Theory]
    [InlineData(TransformerType.Handlebars)]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public async Task Response_ProvideResponse_Transformer_ResultAsJArray(TransformerType transformerType)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "GET", ClientIp);

        var array = JArray.Parse("[{\"x\":\"test\"}]");
        var responseBuilder = Response.Create()
            .WithBodyAsJson(array)
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson).Should().Be("[{\"x\":\"test\"}]");
    }

    //[Theory]
    //[InlineData(TransformerType.Handlebars, "a")]
    //[InlineData(TransformerType.Handlebars, "42")]
    //[InlineData(TransformerType.Handlebars, "{")]
    //[InlineData(TransformerType.Handlebars, "]")]
    //[InlineData(TransformerType.Handlebars, " ")]
    //public async Task Response_ProvideResponse_Transformer_WithBodyAsJsonWithExtraQuotes_AndSpecialOption_MakesAString_ResultAsObject(TransformerType transformerType, string text)
    //{
    //    string jsonString = $"{{ \"x\": \"{text}\" }}";
    //    var bodyData = new BodyData
    //    {
    //        BodyAsJson = JsonConvert.DeserializeObject(jsonString),
    //        DetectedBodyType = BodyType.Json,
    //        Encoding = Encoding.UTF8
    //    };
    //    var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

    //    var responseBuilder = Response.Create()
    //        .WithBodyAsJson(new { text = "\"{{request.bodyAsJson.x}}\"" })
    //        .WithTransformer(transformerType, false, ReplaceNodeOptions.Default);

    //    // Act
    //    var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request,  _settings).ConfigureAwait(false);

    //    // Assert
    //    JsonConvert.SerializeObject(response.Message.BodyData.BodyAsJson).Should().Be($"{{\"text\":\"{text}\"}}");
    //}

    [Theory]
    [InlineData(TransformerType.Handlebars, "\"\"", "\"\"")]
    [InlineData(TransformerType.Handlebars, "\"a\"", "\"a\"")]
    [InlineData(TransformerType.Handlebars, "\" \"", "\" \"")]
    [InlineData(TransformerType.Handlebars, "\"'\"", "\"'\"")]
    [InlineData(TransformerType.Handlebars, "\"false\"", "false")] // bool is special
    [InlineData(TransformerType.Handlebars, "false", "false")]
    [InlineData(TransformerType.Handlebars, "\"true\"", "true")] // bool is special
    [InlineData(TransformerType.Handlebars, "true", "true")]
    [InlineData(TransformerType.Handlebars, "\"-42\"", "-42")] // todo
    [InlineData(TransformerType.Handlebars, "-42", "-42")]
    [InlineData(TransformerType.Handlebars, "\"2147483647\"", "2147483647")] // todo
    [InlineData(TransformerType.Handlebars, "2147483647", "2147483647")]
    [InlineData(TransformerType.Handlebars, "\"9223372036854775807\"", "9223372036854775807")] // todo
    [InlineData(TransformerType.Handlebars, "9223372036854775807", "9223372036854775807")]
    public async Task Response_ProvideResponse_Transformer_WithBodyAsJson_And_ReplaceNodeOptionsKeep(TransformerType transformerType, string value, string expected)
    {
        string jsonString = $"{{ \"x\": {value} }}";
        var bodyData = new BodyData
        {
            BodyAsJson = JsonConvert.DeserializeObject(jsonString),
            DetectedBodyType = BodyType.Json,
            Encoding = Encoding.UTF8
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { text = "{{request.bodyAsJson.x}}" })
            .WithTransformer(transformerType, false, ReplaceNodeOptions.None);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson).Should().Be($"{{\"text\":{expected}}}");
    }

    [Theory]
    [InlineData(TransformerType.Handlebars, "\"\"", "\"\"")]
    [InlineData(TransformerType.Handlebars, "\"a\"", "\"a\"")]
    [InlineData(TransformerType.Handlebars, "\" \"", "\" \"")]
    [InlineData(TransformerType.Handlebars, "\"'\"", "\"'\"")]
    [InlineData(TransformerType.Handlebars, "\"false\"", "false")] // bool is special
    [InlineData(TransformerType.Handlebars, "false", "false")]
    [InlineData(TransformerType.Handlebars, "\"true\"", "true")] // bool is special
    [InlineData(TransformerType.Handlebars, "true", "true")]
    [InlineData(TransformerType.Handlebars, "\"-42\"", "\"-42\"")]
    [InlineData(TransformerType.Handlebars, "-42", "\"-42\"")]
    [InlineData(TransformerType.Handlebars, "\"2147483647\"", "\"2147483647\"")]
    [InlineData(TransformerType.Handlebars, "2147483647", "\"2147483647\"")]
    [InlineData(TransformerType.Handlebars, "\"9223372036854775807\"", "\"9223372036854775807\"")]
    [InlineData(TransformerType.Handlebars, "9223372036854775807", "\"9223372036854775807\"")]
    public async Task Response_ProvideResponse_Transformer_WithBodyAsJsonWithExtraQuotes_AlwaysMakesString(TransformerType transformerType, string value, string expected)
    {
        string jsonString = $"{{ \"x\": {value} }}";
        var bodyData = new BodyData
        {
            BodyAsJson = JsonConvert.DeserializeObject(jsonString),
            DetectedBodyType = BodyType.Json,
            Encoding = Encoding.UTF8
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { text = "\"{{request.bodyAsJson.x}}\"" })
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson).Should().Be($"{{\"text\":{expected}}}");
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

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new[] { "first", "{{request.path}}", "{{request.bodyAsJson.a}}", "{{request.bodyAsJson.b}}", "last" })
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson)).Equals("[\"first\",\"/foo_array\",\"test 1\",\"test 2\",\"last\"]");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_WithBodyAsFile()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo?MyUniqueNumber=1"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithTransformer()
            .WithBodyFromFile(@"c:\\{{request.query.MyUniqueNumber}}\\test.xml");

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsFile).Equals(@"c:\1\test.xml");
    }

    [Theory(Skip = @"Does not work in Scriban --> c:\\[""1""]\\test.xml")]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public async Task Response_ProvideResponse_Scriban_WithBodyAsFile(TransformerType transformerType)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo?MyUniqueNumber=1"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithTransformer(transformerType)
            .WithBodyFromFile(@"c:\\{{request.query.MyUniqueNumber}}\\test.xml");

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsFile).Equals(@"c:\1\test.xml");
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

        var responseBuilder = Response.Create()
            .WithTransformer(transformerType, true)
            .WithBodyFromFile(@"c:\\{{request.query.MyUniqueNumber}}\\test.xml");

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData!.BodyAsFile).Equals(@"c:\1\test.xml");
        Check.That(response.Message.BodyData.DetectedBodyType).Equals(BodyType.String);
        Check.That(response.Message.BodyData!.BodyAsString).Equals("<xml MyUniqueNumber=\"1\"></xml>");
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

        var responseBuilder = Response.Create()
            .WithBodyAsJson("test")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson)).Equals("\"test\"");
    }

    [Fact(Skip = "todo...")]
    //[Fact]
    public async Task Response_ProvideResponse_Handlebars_WithBodyAsJson_ResultAsTemplatedString()
    {
        // Assign
        string jsonString = "{ \"name\": \"WireMock\", \"id\": 12345 }";
        var bodyData = new BodyData
        {
            BodyAsJson = JsonConvert.DeserializeObject(jsonString),
            DetectedBodyType = BodyType.Json,
            Encoding = Encoding.UTF8
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo_object"), "POST", ClientIp, bodyData);

        var responseBuilder = Response.Create()
            .WithBodyAsJson("{{{request.BodyAsJson.name}}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson)).Equals("{\"name\":\"WireMock\"}");
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

        var responseBuilder = Response.Create()
            .WithBodyAsJson("{{{request.BodyAsJson}}}")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson)).Equals("{\"name\":\"WireMock\"}");
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

        var responseBuilder = Response.Create()
            .WithBody("{{request.Body}}", BodyDestinationFormat.SameAsSource, enc)
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Be(text);
        response.Message.BodyData.Encoding.Should().Be(enc);
    }
}