// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using FluentAssertions;
using HandlebarsDotNet;
using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsLinqTests
{
    private readonly WireMockServerSettings _settings = new();

    private Mock<IMapping> mappingMock;

    public ResponseWithHandlebarsLinqTests()
    {
        mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_Linq1_String0()
    {
        // Assign
        var body = new BodyData();

        var request = new RequestMessage(new UrlDetails("http://localhost:1234/pathtest"), "POST", "::1", body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBodyAsJson(new { x = "{{Linq request.Path 'it'}}" })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        Check.That(j["x"]).IsNotNull();
        Check.That(j["x"].ToString()).Equals("/pathtest");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_Linq1_String1()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test") }
            },
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson 'it.Name + \"_123\"' }}" })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        Check.That(j["x"]).IsNotNull();
        Check.That(j["x"].ToString()).Equals("Test_123");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_Linq1_String2()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test") }
            },
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson 'new(it.Name + \"_123\" as N, it.Id as I)' }}" })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        Check.That(j["x"]).IsNotNull();
        Check.That(j["x"].ToString()).Equals("{ N = Test_123, I = 9 }");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_Linq2_Object()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test") }
            },
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBodyAsJson(new { x = "{{#Linq request.bodyAsJson 'new(it.Name + \"_123\" as N, it.Id as I)' }}{{this}}{{/Linq}}" })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        Check.That(j["x"]).IsNotNull();
        Check.That(j["x"].ToString()).Equals("{ N = Test_123, I = 9 }");
    }

    [Fact]
    public void Response_ProvideResponse_Handlebars_Linq_Throws_ArgumentException()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new { x = "x" },
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson 1}}" })
            .WithTransformer();

        // Act
        Check.ThatAsyncCode(() => responseBuilder.ProvideResponseAsync(mappingMock.Object, request, _settings)).Throws<ArgumentException>();
    }

    [Fact]
    public void Response_ProvideResponse_Handlebars_Linq1_Throws_HandlebarsException()
    {
        // Assign
        var body = new BodyData();

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson}} ''" })
            .WithTransformer();

        // Act
        Check.ThatAsyncCode(() => responseBuilder.ProvideResponseAsync(mappingMock.Object, request, _settings)).Throws<HandlebarsException>();
    }

    [Fact]
    public void Response_ProvideResponse_Handlebars_Linq1_ParseError_Throws_ExceptionMessage()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test") }
            },
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson '---' }}" })
            .WithTransformer();

        // Act
        Func<Task> a = async () => await responseBuilder.ProvideResponseAsync(mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        a.Should().ThrowAsync<HandlebarsException>();
    }

    [Fact]
    public void Response_ProvideResponse_Handlebars_Linq2_ParseError_Throws_ExceptionMessage()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test") }
            },
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { x = "{{#Linq request.bodyAsJson '---' }}{{this}}{{/Linq}}" })
            .WithTransformer();

        // Act
        Func<Task> a = async () => await responseBuilder.ProvideResponseAsync(mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        a.Should().ThrowAsync<HandlebarsException>();
    }
}