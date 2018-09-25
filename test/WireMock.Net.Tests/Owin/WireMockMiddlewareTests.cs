using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Collections.Generic;
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

        private Mock<IWireMockMiddlewareOptions> _options;
        private Mock<IOwinRequestMapper> _requestMapper;
        private Mock<IOwinResponseMapper> _responseMapper;
        private Mock<IMappingMatcher> _matcher;
        private Mock<IContext> _context;

        public WireMockMiddlewareTests()
        {
            _options = new Mock<IWireMockMiddlewareOptions>();
            _options.SetupAllProperties();
            _options.Setup(o => o.Mappings).Returns(new ConcurrentDictionary<Guid, Mapping>());
            _options.Setup(o => o.LogEntries).Returns(new ConcurentObservableCollection<LogEntry>());
            _options.Setup(o => o.Scenarios).Returns(new ConcurrentDictionary<string, ScenarioState>());
            _options.Setup(o => o.Logger.Warn(It.IsAny<string>(), It.IsAny<object[]>()));
            _options.Setup(o => o.Logger.DebugRequestResponse(It.IsAny<LogEntryModel>(), false));

            _requestMapper = new Mock<IOwinRequestMapper>();
            _requestMapper.SetupAllProperties();

            _responseMapper = new Mock<IOwinResponseMapper>();
            _responseMapper.SetupAllProperties();

            _matcher = new Mock<IMappingMatcher>();
            _matcher.SetupAllProperties();
            _matcher.Setup(m => m.Match(It.IsAny<RequestMessage>())).Returns(((Mapping)null, (RequestMatchResult)null));

            _context = new Mock<IContext>();

            _sut = new WireMockMiddleware(null, _options.Object, _requestMapper.Object, _responseMapper.Object, _matcher.Object);
        }

        [Fact]
        public async void WireMockMiddleware_Invoke_NoMatch()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/json" } } };
            var body = new BodyData
            {
                BodyAsJson = new { x = 1 }
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", body, headers);

            _requestMapper.Setup(m => m.MapAsync(It.IsAny<IRequest>())).ReturnsAsync(request);
            _responseMapper.Setup(m => m.MapAsync(It.IsAny<ResponseMessage>(), It.IsAny<IResponse>())).Returns(Task.FromResult(true));

            // Act
            await _sut.Invoke(_context.Object);

            // Assert and Verify
            _options.Verify(o => o.Logger.Warn(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

            Expression<Func<ResponseMessage, bool>> match = r => r.StatusCode == 404 && ((StatusModel)r.BodyAsJson).Status == "No matching mapping found";
            _responseMapper.Verify(m => m.MapAsync(It.Is(match), It.IsAny<IResponse>()), Times.Once);
        }
    }
}