using System.Collections.Generic;
using System.IO;
using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Threading;
using WireMock.Handlers;
using WireMock.Owin.Mappers;
using WireMock.ResponseBuilders;
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

namespace WireMock.Net.Tests.Owin.Mappers
{
    public class OwinResponseMapperTests
    {
        private static readonly Task CompletedTask = Task.FromResult(true);
        private readonly OwinResponseMapper _sut;
        private readonly Mock<IResponse> _responseMock;
        private readonly Mock<Stream> _stream;
        private readonly Mock<IHeaderDictionary> _headers;
        private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock;

        public OwinResponseMapperTests()
        {
            _stream = new Mock<Stream>();
            _stream.SetupAllProperties();
            _stream.Setup(s => s.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(CompletedTask);

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

            _fileSystemHandlerMock = new Mock<IFileSystemHandler>();
            _fileSystemHandlerMock.SetupAllProperties();

            _sut = new OwinResponseMapper(_fileSystemHandlerMock.Object);
        }

        [Fact]
        public async Task OwinResponseMapper_MapAsync_Null()
        {
            // Act
            await _sut.MapAsync(null, _responseMock.Object);
        }

        [Fact]
        public async Task OwinResponseMapper_MapAsync_StatusCode()
        {
            // Arrange
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
        public async Task OwinResponseMapper_MapAsync_NoBody()
        {
            // Arrange
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
        public async Task OwinResponseMapper_MapAsync_Body()
        {
            // Arrange
            string body = "abcd";
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = body }
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _stream.Verify(s => s.WriteAsync(new byte[] { 97, 98, 99, 100 }, 0, 4, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OwinResponseMapper_MapAsync_BodyAsBytes()
        {
            // Arrange
            var bytes = new byte[] { 48, 49 };
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                BodyData = new BodyData { DetectedBodyType = BodyType.Bytes, BodyAsBytes = bytes }
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _stream.Verify(s => s.WriteAsync(bytes, 0, bytes.Length, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OwinResponseMapper_MapAsync_BodyAsJson()
        {
            // Arrange
            var json = new { t = "x", i = (string)null };
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                BodyData = new BodyData { DetectedBodyType = BodyType.Json, BodyAsJson = json, BodyAsJsonIndented = false }
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _stream.Verify(s => s.WriteAsync(new byte[] { 123, 34, 116, 34, 58, 34, 120, 34, 125 }, 0, 9, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OwinResponseMapper_MapAsync_SetResponseHeaders()
        {
            // Arrange
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>> { { "h", new WireMockList<string>("x", "y") } }
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
#if NET452
            _headers.Verify(h => h.AppendValues("h", new string[] { "x", "y" }), Times.Once);
#else
            var v = new StringValues();
            _headers.Verify(h => h.TryGetValue("h", out v), Times.Once);
#endif
        }

        [Fact]
        public async Task OwinResponseMapper_MapAsync_WithFault_EMPTY_RESPONSE()
        {
            // Arrange
            string body = "abc";
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = body },
                FaultType = FaultType.EMPTY_RESPONSE
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _stream.Verify(s => s.WriteAsync(new byte[0], 0, 0, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OwinResponseMapper_MapAsync_WithFault_MALFORMED_RESPONSE_CHUNK()
        {
            // Arrange
            string body = "abcd";
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = body },
                StatusCode = 100,
                FaultType = FaultType.MALFORMED_RESPONSE_CHUNK
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _responseMock.VerifySet(r => r.StatusCode = 100, Times.Once);
            _stream.Verify(s => s.WriteAsync(It.Is<byte[]>(bytes => bytes[0] == 97 && bytes[1] == 98), 0, It.Is<int>(count => count >= 4), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OwinResponseMapper_MapAsync_WithFault_RANDOM_DATA_THEN_CLOSE()
        {
            // Arrange
            string body = "abcd";
            var responseMessage = new ResponseMessage
            {
                Headers = new Dictionary<string, WireMockList<string>>(),
                BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = body },
                StatusCode = 100,
                FaultType = FaultType.RANDOM_DATA_THEN_CLOSE
            };

            // Act
            await _sut.MapAsync(responseMessage, _responseMock.Object);

            // Assert
            _responseMock.VerifySet(r => r.StatusCode = 100, Times.Never);
            _stream.Verify(s => s.WriteAsync(It.IsAny<byte[]>(), 0, It.Is<int>(count => count >= 4), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}