//using Microsoft.Owin;
//using Moq;
//using NFluent;
//using WireMock.Owin;
//using Xunit;

//namespace WireMock.Net.Tests
//{
//    public class WireMockMiddlewareTests
//    {
//        private readonly ObjectMother _objectMother = new ObjectMother();

//        [Fact]
//        public void Should_have_default_state_as_null()
//        {
//            // given

//            // when
//            var sut = _objectMother.Create();

//            // then
//            Check.That(sut.States).IsNull();
//        }

//        private class ObjectMother
//        {
//            private Mock<OwinMiddleware> OwinMiddleware { get; }
//            private Mock<IOwinContext> OwinContext { get; }
//            private WireMockMiddlewareOptions WireMockMiddlewareOptions { get; }

//            public ObjectMother()
//            {
//                OwinContext = new Mock<IOwinContext>();
//                OwinMiddleware = new Mock<OwinMiddleware>(null);
//                WireMockMiddlewareOptions = new WireMockMiddlewareOptions();
//            }

//            public WireMockMiddleware Create()
//            {
//                return new WireMockMiddleware(OwinMiddleware.Object, WireMockMiddlewareOptions);
//            }
//        }
//    }
//}