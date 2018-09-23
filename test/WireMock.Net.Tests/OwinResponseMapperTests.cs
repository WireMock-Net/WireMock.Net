using NFluent;
using WireMock.Owin;
using Xunit;
using Moq;
#if NET452
using Microsoft.Owin;
using IResponse = Microsoft.Owin.IOwinResponse;
using Response = Microsoft.Owin.OwinResponse;
#else
using Microsoft.AspNetCore.Http;
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
using Response = Microsoft.AspNetCore.Http.HttpResponse;
#endif

namespace WireMock.Net.Tests
{
    public class OwinResponseMapperTests
    {
        private OwinResponseMapper _sut;
        private Mock<IResponse> _responseMock;

        public OwinResponseMapperTests()
        {
            _responseMock = new Mock<IResponse>();
            _responseMock.SetupAllProperties();

            _sut = new OwinResponseMapper();
        }

        [Fact]
        public async void OwinResponseMapper_MapAsync_Null()
        {
            // Act
            await _sut.MapAsync(null, _responseMock.Object);
        }

        [Fact]
        public async void OwinResponseMapper_MapAsync_StatusCode()
        {
            // Assign
            var responseMessage = new ResponseMessage
            {
                StatusCode = 302
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _responseMock.VerifySet(r => r.StatusCode = 302, Times.Once);
        }
    }
}