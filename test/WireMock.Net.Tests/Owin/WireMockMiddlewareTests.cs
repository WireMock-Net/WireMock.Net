// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Xunit;
using WireMock.Models;
using WireMock.Owin;
using WireMock.Owin.Mappers;
using WireMock.Util;
using WireMock.Logging;
using WireMock.Matchers;
using System.Collections.Generic;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Settings;
using FluentAssertions;
using WireMock.Handlers;
using WireMock.Matchers.Request;
using WireMock.ResponseBuilders;
using WireMock.RequestBuilders;
#if NET452
using Microsoft.Owin;
using IContext = Microsoft.Owin.IOwinContext;
using IRequest = Microsoft.Owin.IOwinRequest;
using IResponse = Microsoft.Owin.IOwinResponse;
#else
using Microsoft.AspNetCore.Http;
using IContext = Microsoft.AspNetCore.Http.HttpContext;
using IRequest = Microsoft.AspNetCore.Http.HttpRequest;
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
#endif

namespace WireMock.Net.Tests.Owin;

public class WireMockMiddlewareTests
{
    private static readonly Guid NewGuid = new("98fae52e-76df-47d9-876f-2ee32e931d9b");
    private static readonly DateTime UpdatedAt = new(2022, 12, 4);

    private readonly ConcurrentDictionary<Guid, IMapping> _mappings = new();
    private readonly Mock<IWireMockMiddlewareOptions> _optionsMock;
    private readonly Mock<IOwinRequestMapper> _requestMapperMock;
    private readonly Mock<IOwinResponseMapper> _responseMapperMock;
    private readonly Mock<IMappingMatcher> _matcherMock;
    private readonly Mock<IMapping> _mappingMock;
    private readonly Mock<IRequestMatchResult> _requestMatchResultMock;
    private readonly Mock<IContext> _contextMock;

    private readonly WireMockMiddleware _sut;

    public WireMockMiddlewareTests()
    {
        var guidUtilsMock = new Mock<IGuidUtils>();
        guidUtilsMock.Setup(g => g.NewGuid()).Returns(NewGuid);

        _optionsMock = new Mock<IWireMockMiddlewareOptions>();
        _optionsMock.SetupAllProperties();
        _optionsMock.Setup(o => o.Mappings).Returns(_mappings);
        _optionsMock.Setup(o => o.LogEntries).Returns(new ConcurrentObservableCollection<LogEntry>());
        _optionsMock.Setup(o => o.Scenarios).Returns(new ConcurrentDictionary<string, ScenarioState>());
        _optionsMock.Setup(o => o.Logger.Warn(It.IsAny<string>(), It.IsAny<object[]>()));
        _optionsMock.Setup(o => o.Logger.Error(It.IsAny<string>(), It.IsAny<object[]>()));
        _optionsMock.Setup(o => o.Logger.DebugRequestResponse(It.IsAny<LogEntryModel>(), It.IsAny<bool>()));

        _requestMapperMock = new Mock<IOwinRequestMapper>();
        _requestMapperMock.SetupAllProperties();
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
        _requestMapperMock.Setup(m => m.MapAsync(It.IsAny<IRequest>(), It.IsAny<IWireMockMiddlewareOptions>())).ReturnsAsync(request);

        _responseMapperMock = new Mock<IOwinResponseMapper>();
        _responseMapperMock.SetupAllProperties();
        _responseMapperMock.Setup(m => m.MapAsync(It.IsAny<ResponseMessage?>(), It.IsAny<IResponse>())).Returns(Task.FromResult(true));

        _matcherMock = new Mock<IMappingMatcher>();
        _matcherMock.SetupAllProperties();
        // _matcherMock.Setup(m => m.FindBestMatch(It.IsAny<RequestMessage>())).Returns((new MappingMatcherResult(), new MappingMatcherResult()));

        _contextMock = new Mock<IContext>();

        _mappingMock = new Mock<IMapping>();

        _requestMatchResultMock = new Mock<IRequestMatchResult>();
        _requestMatchResultMock.Setup(r => r.TotalNumber).Returns(1);
        _requestMatchResultMock.Setup(r => r.MatchDetails).Returns(new List<MatchDetail>());

        _sut = new WireMockMiddleware(
            null,
            _optionsMock.Object,
            _requestMapperMock.Object,
            _responseMapperMock.Object,
            _matcherMock.Object,
            guidUtilsMock.Object
        );
    }

    [Fact]
    public async Task WireMockMiddleware_Invoke_NoMatch()
    {
        // Act
        await _sut.Invoke(_contextMock.Object).ConfigureAwait(false);

        // Assert and Verify
        _optionsMock.Verify(o => o.Logger.Warn(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

        Expression<Func<ResponseMessage, bool>> match = r => (int)r.StatusCode! == 404 && ((StatusModel)r.BodyData!.BodyAsJson!).Status == "No matching mapping found";
        _responseMapperMock.Verify(m => m.MapAsync(It.Is(match), It.IsAny<IResponse>()), Times.Once);
    }

    [Fact]
    public async Task WireMockMiddleware_Invoke_NoMatch_When_SaveUnmatchedRequestsIsTrue_Should_Call_LocalFileSystemHandler_WriteUnmatchedRequest()
    {
        // Arrange
        var fileSystemHandlerMock = new Mock<IFileSystemHandler>();
        _optionsMock.Setup(o => o.FileSystemHandler).Returns(fileSystemHandlerMock.Object);
        _optionsMock.Setup(o => o.SaveUnmatchedRequests).Returns(true);

        // Act
        await _sut.Invoke(_contextMock.Object).ConfigureAwait(false);

        // Assert
        _optionsMock.Verify(o => o.Logger.Warn(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

        Expression<Func<ResponseMessage, bool>> match = r => (int)r.StatusCode! == 404 && ((StatusModel)r.BodyData!.BodyAsJson!).Status == "No matching mapping found";
        _responseMapperMock.Verify(m => m.MapAsync(It.Is(match), It.IsAny<IResponse>()), Times.Once);

        // Verify
        fileSystemHandlerMock.Verify(f => f.WriteUnmatchedRequest("98fae52e-76df-47d9-876f-2ee32e931d9b.LogEntry.json", It.IsAny<string>()));
        fileSystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WireMockMiddleware_Invoke_IsAdminInterface_EmptyHeaders_401()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", null, new Dictionary<string, string[]>());
        _requestMapperMock.Setup(m => m.MapAsync(It.IsAny<IRequest>(), It.IsAny<IWireMockMiddlewareOptions>())).ReturnsAsync(request);

        _optionsMock.SetupGet(o => o.AuthenticationMatcher).Returns(new ExactMatcher());
        _mappingMock.SetupGet(m => m.IsAdminInterface).Returns(true);

        var result = new MappingMatcherResult(_mappingMock.Object, _requestMatchResultMock.Object);
        _matcherMock.Setup(m => m.FindBestMatch(It.IsAny<RequestMessage>())).Returns((result, result));

        // Act
        await _sut.Invoke(_contextMock.Object).ConfigureAwait(false);

        // Assert and Verify
        _optionsMock.Verify(o => o.Logger.Error(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

        Expression<Func<ResponseMessage, bool>> match = r => (int?)r.StatusCode == 401;
        _responseMapperMock.Verify(m => m.MapAsync(It.Is(match), It.IsAny<IResponse>()), Times.Once);
    }

    [Fact]
    public async Task WireMockMiddleware_Invoke_IsAdminInterface_MissingHeader_401()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", null, new Dictionary<string, string[]> { { "h", new[] { "x" } } });
        _requestMapperMock.Setup(m => m.MapAsync(It.IsAny<IRequest>(), It.IsAny<IWireMockMiddlewareOptions>())).ReturnsAsync(request);

        _optionsMock.SetupGet(o => o.AuthenticationMatcher).Returns(new ExactMatcher());
        _mappingMock.SetupGet(m => m.IsAdminInterface).Returns(true);

        var result = new MappingMatcherResult(_mappingMock.Object, _requestMatchResultMock.Object);
        _matcherMock.Setup(m => m.FindBestMatch(It.IsAny<RequestMessage>())).Returns((result, result));

        // Act
        await _sut.Invoke(_contextMock.Object).ConfigureAwait(false);

        // Assert and Verify
        _optionsMock.Verify(o => o.Logger.Error(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

        Expression<Func<ResponseMessage, bool>> match = r => (int?)r.StatusCode == 401;
        _responseMapperMock.Verify(m => m.MapAsync(It.Is(match), It.IsAny<IResponse>()), Times.Once);
    }

    [Fact]
    public async Task WireMockMiddleware_Invoke_RequestLogExpirationDurationIsDefined()
    {
        // Assign
        _optionsMock.SetupGet(o => o.RequestLogExpirationDuration).Returns(1);

        // Act
        await _sut.Invoke(_contextMock.Object).ConfigureAwait(false);
    }

    [Fact]
    public async Task WireMockMiddleware_Invoke_Mapping_Has_ProxyAndRecordSettings_And_SaveMapping_Is_True()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", null, new Dictionary<string, string[]>());
        _requestMapperMock.Setup(m => m.MapAsync(It.IsAny<IRequest>(), It.IsAny<IWireMockMiddlewareOptions>())).ReturnsAsync(request);

        _optionsMock.SetupGet(o => o.AuthenticationMatcher).Returns(new ExactMatcher());

        var fileSystemHandlerMock = new Mock<IFileSystemHandler>();
        fileSystemHandlerMock.Setup(f => f.GetMappingFolder()).Returns("m");

        var logger = new Mock<IWireMockLogger>();

        var proxyAndRecordSettings = new ProxyAndRecordSettings
        {
            SaveMapping = true,
            SaveMappingToFile = true
        };

        var settings = new WireMockServerSettings
        {
            FileSystemHandler = fileSystemHandlerMock.Object,
            Logger = logger.Object
        };

        var responseBuilder = Response.Create().WithProxy(proxyAndRecordSettings);

        _mappingMock.SetupGet(m => m.Provider).Returns(responseBuilder);
        _mappingMock.SetupGet(m => m.Settings).Returns(settings);

        var newMappingFromProxy = new Mapping(NewGuid, UpdatedAt, string.Empty, string.Empty, null, settings, Request.Create(), Response.Create(), 0, null, null, null, null, null, false, null, null);
        _mappingMock.Setup(m => m.ProvideResponseAsync(It.IsAny<RequestMessage>())).ReturnsAsync((new ResponseMessage(), newMappingFromProxy));

        var requestBuilder = Request.Create().UsingAnyMethod();
        _mappingMock.SetupGet(m => m.RequestMatcher).Returns(requestBuilder);

        var result = new MappingMatcherResult(_mappingMock.Object, _requestMatchResultMock.Object);
        _matcherMock.Setup(m => m.FindBestMatch(It.IsAny<RequestMessage>())).Returns((result, result));

        // Act
        await _sut.Invoke(_contextMock.Object).ConfigureAwait(false);

        // Assert and Verify
        fileSystemHandlerMock.Verify(f => f.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _mappings.Count.Should().Be(1);
    }

    [Fact]
    public async Task WireMockMiddleware_Invoke_Mapping_Has_ProxyAndRecordSettings_And_SaveMapping_Is_False_But_WireMockServerSettings_SaveMapping_Is_True()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", null, new Dictionary<string, string[]>());
        _requestMapperMock.Setup(m => m.MapAsync(It.IsAny<IRequest>(), It.IsAny<IWireMockMiddlewareOptions>())).ReturnsAsync(request);

        _optionsMock.SetupGet(o => o.AuthenticationMatcher).Returns(new ExactMatcher());

        var fileSystemHandlerMock = new Mock<IFileSystemHandler>();
        fileSystemHandlerMock.Setup(f => f.GetMappingFolder()).Returns("m");

        var logger = new Mock<IWireMockLogger>();

        var proxyAndRecordSettings = new ProxyAndRecordSettings
        {
            SaveMapping = false,
            SaveMappingToFile = false
        };

        var settings = new WireMockServerSettings
        {
            FileSystemHandler = fileSystemHandlerMock.Object,
            Logger = logger.Object,
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                SaveMapping = true,
                SaveMappingToFile = true
            }
        };

        var responseBuilder = Response.Create().WithProxy(proxyAndRecordSettings);

        _mappingMock.SetupGet(m => m.Provider).Returns(responseBuilder);
        _mappingMock.SetupGet(m => m.Settings).Returns(settings);

        var newMappingFromProxy = new Mapping(NewGuid, UpdatedAt, "my-title", "my-description", null, settings, Request.Create(), Response.Create(), 0, null, null, null, null, null, false, null, data: null);
        _mappingMock.Setup(m => m.ProvideResponseAsync(It.IsAny<RequestMessage>())).ReturnsAsync((new ResponseMessage(), newMappingFromProxy));

        var requestBuilder = Request.Create().UsingAnyMethod();
        _mappingMock.SetupGet(m => m.RequestMatcher).Returns(requestBuilder);

        var result = new MappingMatcherResult (_mappingMock.Object, _requestMatchResultMock.Object);
        _matcherMock.Setup(m => m.FindBestMatch(It.IsAny<RequestMessage>())).Returns((result, result));

        // Act
        await _sut.Invoke(_contextMock.Object).ConfigureAwait(false);

        // Assert and Verify
        fileSystemHandlerMock.Verify(f => f.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _mappings.Should().HaveCount(1);
    }
}