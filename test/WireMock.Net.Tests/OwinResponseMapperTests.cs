using System.Collections.Generic;
using System.IO;
using WireMock.Owin;
using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Threading;
using WireMock.Util;
#if NET452
using Microsoft.Owin;
using IResponse = Microsoft.Owin.IOwinResponse;
using Response = Microsoft.Owin.OwinResponse;
#else
using Microsoft.AspNetCore.Http;
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
using Response = Microsoft.AspNetCore.Http.HttpResponse;
using Microsoft.Extensions.Primitives;
#endif

namespace WireMock.Net.Tests
{
    public class OwinResponseMapperTests
    {
        private static Task completedTask = Task.FromResult(true);
        private OwinResponseMapper _sut;
        private Mock<IResponse> _responseMock;
        private Mock<Stream> _stream;
        private Mock<IHeaderDictionary> _headers;

        public OwinResponseMapperTests()
        {
            _stream = new Mock<Stream>();
            _stream.SetupAllProperties();
            _stream.Setup(s => s.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(completedTask);

            _headers = new Mock<IHeaderDictionary>();
            _headers.SetupAllProperties();
#if NET452
            _headers.Setup(h => h.AppendValues(It.IsAny<string>(), It.IsAny<string[]>()));
#else
            _headers.Setup(h => h.Add(It.IsAny<string>(), It.IsAny<StringValues>()));
#endif

            _responseMock = new Mock<IResponse>();
            _responseMock.SetupAllProperties();
            _responseMock.SetupGet(r => r.Body).Returns(_stream.Object);
            _responseMock.SetupGet(r => r.Headers).Returns(_headers.Object);

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

        [Fact]
        public async void OwinResponseMapper_MapAsync_NoBody()
        {
            // Assign
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>()
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _stream.Verify(s => s.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async void OwinResponseMapper_MapAsync_Body()
        {
            // Assign
            string body = "abc";
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                Body = body
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _stream.Verify(s => s.WriteAsync(new byte[] { 97, 98, 99 }, 0, 3, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void OwinResponseMapper_MapAsync_BodyAsBytes()
        {
            // Assign
            var bytes = new byte[] { 48, 49 };
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                BodyAsBytes = bytes
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _stream.Verify(s => s.WriteAsync(bytes, 0, bytes.Length, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void OwinResponseMapper_MapAsync_BodyAsJson()
        {
            // Assign
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                BodyAsJson = new { t = "x", i = (string)null },
                BodyAsJsonIndented = false
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _stream.Verify(s => s.WriteAsync(new byte[] { 123, 34, 116, 34, 58, 34, 120, 34, 125 }, 0, 9, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void OwinResponseMapper_MapAsync_SetResponseHeaders()
        {
            // Assign
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>> { { "h", new WireMockList<string>("x", "y") } }
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
#if NET452
            _headers.Verify(h => h.AppendValues("h", new string[] { "x", "y" } ), Times.Once);
#else
            var v = new StringValues();
            _headers.Verify(h => h.TryGetValue("h", out v), Times.Once);
#endif
        }
    }
}