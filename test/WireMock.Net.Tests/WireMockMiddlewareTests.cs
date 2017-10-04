using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Moq;
using NFluent;
using WireMock.Owin;
using Xunit;

namespace WireMock.Net.Tests
{
    public class WireMockMiddlewareTests
    {
        private readonly ObjectMother _objectMother = new ObjectMother();

        [Fact]
        public void Should_have_default_state_as_null()
        {
            // given

            // when
            var sut = _objectMother.Create();

            // then
            Check.That(sut.State).IsNull();
        }

        internal class ObjectMother
        {
            public Mock<OwinMiddleware> OwinMiddleware { get; set; }
            public Mock<IOwinContext> OwinContext { get; set; }
            public WireMockMiddlewareOptions WireMockMiddlewareOptions { get; set; }

            public ObjectMother()
            {
                OwinContext = new Mock<IOwinContext>();
                OwinMiddleware = new Mock<OwinMiddleware>(null);
                WireMockMiddlewareOptions = new WireMockMiddlewareOptions();
            }

            public WireMockMiddleware Create()
            {
                return new WireMockMiddleware(OwinMiddleware.Object, WireMockMiddlewareOptions);
            }
        }
    }
}
