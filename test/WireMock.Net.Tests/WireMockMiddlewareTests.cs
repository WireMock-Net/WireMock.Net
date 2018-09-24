using Moq;
using NFluent;
using WireMock.Owin;
using Xunit;
#if NET452
using Microsoft.Owin;
using IContext = Microsoft.Owin.IOwinContext;
using OwinMiddleware = Microsoft.Owin.OwinMiddleware;
using Next = Microsoft.Owin.OwinMiddleware;
#else
using Microsoft.AspNetCore.Http;
using OwinMiddleware = System.Object;
using IContext = Microsoft.AspNetCore.Http.HttpContext;
using Next = Microsoft.AspNetCore.Http.RequestDelegate;
#endif

namespace WireMock.Net.Tests
{
    public class WireMockMiddlewareTests
    {
        //private Mock<OwinMiddleware> OwinMiddleware;
        //private Mock<IOwinContext> OwinContext;
        private WireMockMiddlewareOptions WireMockMiddlewareOptions;

        private WireMockMiddleware _sut;

        public WireMockMiddlewareTests()
        {
            //OwinContext = new Mock<IOwinContext>();
            //OwinMiddleware = new Mock<OwinMiddleware>(null);
            WireMockMiddlewareOptions = new WireMockMiddlewareOptions();
        }

        [Fact]
        public void WireMockMiddleware_Invoke()
        {
        }
    }
}