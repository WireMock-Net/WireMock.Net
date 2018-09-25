using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Xunit;
using WireMock.Admin.Mappings;
using WireMock.Models;
using WireMock.Owin;
using WireMock.Owin.Mappers;
using WireMock.Util;
using WireMock.Admin.Requests;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Matchers;
using System.Collections.Generic;
#if NET452
using Microsoft.Owin;
using IContext = Microsoft.Owin.IOwinContext;
using OwinMiddleware = Microsoft.Owin.OwinMiddleware;
using Next = Microsoft.Owin.OwinMiddleware;
using IRequest = Microsoft.Owin.IOwinRequest;
using IResponse = Microsoft.Owin.IOwinResponse;
#else
using Microsoft.AspNetCore.Http;
using OwinMiddleware = System.Object;
using IContext = Microsoft.AspNetCore.Http.HttpContext;
using Next = Microsoft.AspNetCore.Http.RequestDelegate;
using IRequest = Microsoft.AspNetCore.Http.HttpRequest;
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
#endif

namespace WireMock.Net.Tests.Owin
{
    public class WireMockMiddlewareTests
    {
        private WireMockMiddleware _sut;

        private Mock<IWireMockMiddlewareOptions> _optionsMock;
        private Mock<IOwinRequestMapper> _requestMapperMock;
        private Mock<IOwinResponseMapper> _responseMapperMock;
        private Mock<IMappingMatcher> _matcherMock;
        private Mock<IMapping> _mappingMock;
        private Mock<IContext> _contextMock;

        public WireMockMiddlewareTests()
        {
            _optionsMock = new Mock<IWireMockMiddlewareOptions>();
            _optionsMock.SetupAllProperties();
            _optionsMock.Setup(o => o.Mappings).Returns(new ConcurrentDictionary<Guid, IMapping>());
            _optionsMock.Setup(o => o.LogEntries).Returns(new ConcurentObservableCollection<LogEntry>());
            _optionsMock.Setup(o => o.Scenarios).Returns(new ConcurrentDictionary<string, ScenarioState>());
            _optionsMock.Setup(o => o.Logger.Warn(It.IsAny<string>(), It.IsAny<object[]>()));
            _optionsMock.Setup(o => o.Logger.Error(It.IsAny<string>(), It.IsAny<object[]>()));
            _optionsMock.Setup(o => o.Logger.DebugRequestResponse(It.IsAny<LogEntryModel>(), It.IsAny<bool>()));

            _requestMapperMock = new Mock<IOwinRequestMapper>();
            _requestMapperMock.SetupAllProperties();
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            _requestMapperMock.Setup(m => m.MapAsync(It.IsAny<IRequest>())).ReturnsAsync(request);

            _responseMapperMock = new Mock<IOwinResponseMapper>();
            _responseMapperMock.SetupAllProperties();
            _responseMapperMock.Setup(m => m.MapAsync(It.IsAny<ResponseMessage>(), It.IsAny<IResponse>())).Returns(Task.FromResult(true));

            _matcherMock = new Mock<IMappingMatcher>();
            _matcherMock.SetupAllProperties();
            _matcherMock.Setup(m => m.Match(It.IsAny<RequestMessage>())).Returns(((IMapping)null, (RequestMatchResult)null));

            _contextMock = new Mock<IContext>();

            _mappingMock = new Mock<IMapping>();

            _sut = new WireMockMiddleware(null, _optionsMock.Object, _requestMapperMock.Object, _responseMapperMock.Object, _matcherMock.Object);
        }

        [Fact]
        public async void WireMockMiddleware_Invoke_NoMatch()
        {
            // Act
            await _sut.Invoke(_contextMock.Object);

            // Assert and Verify
            _optionsMock.Verify(o => o.Logger.Warn(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

            Expression<Func<ResponseMessage, bool>> match = r => r.StatusCode == 404 && ((StatusModel)r.BodyAsJson).Status == "No matching mapping found";
            _responseMapperMock.Verify(m => m.MapAsync(It.Is(match), It.IsAny<IResponse>()), Times.Once);
        }

        [Fact]
        public async void WireMockMiddleware_Invoke_IsAdminInterface_EmptyHeaders_401()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", null, new Dictionary<string, string[]>());
            _requestMapperMock.Setup(m => m.MapAsync(It.IsAny<IRequest>())).ReturnsAsync(request);

            _optionsMock.SetupGet(o => o.AuthorizationMatcher).Returns(new ExactMatcher());
            _mappingMock.SetupGet(m => m.IsAdminInterface).Returns(true);
            _matcherMock.Setup(m => m.Match(It.IsAny<RequestMessage>())).Returns((_mappingMock.Object, (RequestMatchResult)null));

            // Act
            await _sut.Invoke(_contextMock.Object);

            // Assert and Verify
            _optionsMock.Verify(o => o.Logger.Error(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

            Expression<Func<ResponseMessage, bool>> match = r => r.StatusCode == 401;
            _responseMapperMock.Verify(m => m.MapAsync(It.Is(match), It.IsAny<IResponse>()), Times.Once);
        }

        [Fact]
        public async void WireMockMiddleware_Invoke_IsAdminInterface_MissingHeader_401()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", null, new Dictionary<string, string[]> { { "h", new[] { "x" } } });
            _requestMapperMock.Setup(m => m.MapAsync(It.IsAny<IRequest>())).ReturnsAsync(request);

            _optionsMock.SetupGet(o => o.AuthorizationMatcher).Returns(new ExactMatcher());
            _mappingMock.SetupGet(m => m.IsAdminInterface).Returns(true);
            _matcherMock.Setup(m => m.Match(It.IsAny<RequestMessage>())).Returns((_mappingMock.Object, (RequestMatchResult)null));

            // Act
            await _sut.Invoke(_contextMock.Object);

            // Assert and Verify
            _optionsMock.Verify(o => o.Logger.Error(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

            Expression<Func<ResponseMessage, bool>> match = r => r.StatusCode == 401;
            _responseMapperMock.Verify(m => m.MapAsync(It.Is(match), It.IsAny<IResponse>()), Times.Once);
        }



        [Fact]
        public async void WireMockMiddleware_Invoke_RequestLogExpirationDurationIsDefined()
        {
            // Assign
            _optionsMock.SetupGet(o => o.RequestLogExpirationDuration).Returns(1);

            // Act
            await _sut.Invoke(_contextMock.Object);
        }
    }
}