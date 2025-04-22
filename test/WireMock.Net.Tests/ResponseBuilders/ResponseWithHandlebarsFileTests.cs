// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using FluentAssertions;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Transformers.Handlebars;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsFileTests
{
    private const string ClientIp = "::1";

    private readonly WireMockServerSettings _settings;
    private readonly Mock<IMapping> _mappingMock;
    private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;

    public ResponseWithHandlebarsFileTests()
    {
        _mappingMock = new Mock<IMapping>();

        _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings = new()
        {
            AllowedCustomHandlebarHelpers = CustomHandlebarHelpers.File,
            FileSystemHandler = _filesystemHandlerMock.Object
        };
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_File()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Data = "{{File \"x.json\"}}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        var j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        Check.That(j["Data"].Value<string>()).Equals("abc");

        // Verify
        _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString("x.json"), Times.Once);
        _filesystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_File_Replace()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234?id=x"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Data = "{{File \"{{request.query.id}}.json\"}}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        var j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        Check.That(j["Data"].Value<string>()).Equals("abc");

        // Verify
        _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString("x.json"), Times.Once);
        _filesystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Response_ProvideResponseAsync_Handlebars_File_WithMissingArgument_Throws_HandlebarsException()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Data = "{{File}}"
            })
            .WithTransformer();

        // Act
        Check.ThatAsyncCode(() => responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings)).Throws<HandlebarsException>();

        // Verify
        _filesystemHandlerMock.Verify(fs => fs.ReadResponseBodyAsString(It.IsAny<string>()), Times.Never);
        _filesystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Response_ProvideResponseAsync_Handlebars_File_NotAllowed_Throws_HandlebarsRuntimeException()
    {
        // Assign
        var settings = new WireMockServerSettings
        {
            AllowedCustomHandlebarHelpers = CustomHandlebarHelpers.None,
            FileSystemHandler = _filesystemHandlerMock.Object
        };
        var request = new RequestMessage(new UrlDetails("http://localhost:1234?id=x"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("{{File \"{{request.query.id}}.json\"}}")
            .WithTransformer();

        // Act
        Func<Task> action = () => responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, settings);

        action.Should().ThrowAsync<HandlebarsRuntimeException>();

        // Verify
        _filesystemHandlerMock.VerifyNoOtherCalls();
    }
}