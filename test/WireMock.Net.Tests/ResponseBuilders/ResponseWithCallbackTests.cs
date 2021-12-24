using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithCallbackTests
    {
        private const string ClientIp = "::1";

        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        public ResponseWithCallbackTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

        [Fact]
        public async Task Response_WithCallbackAsync()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var responseBuilder = Response.Create()
                .WithCallback(async request =>
                {
                    await Task.Delay(1);

                    return new ResponseMessage
                    {
                        BodyData = new BodyData
                        {
                            DetectedBodyType = BodyType.String,
                            BodyAsString = request.Path + "Bar"
                        },
                        StatusCode = 302
                    };
                });

            // Act
            var response = await responseBuilder.ProvideResponseAsync(requestMessage, _settings).ConfigureAwait(false);

            // Assert
            response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
            response.Message.StatusCode.Should().Be(302);
        }

        [Fact]
        public async Task Response_WithCallback()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var responseBuilder = Response.Create()
                .WithCallback(request => new ResponseMessage
                {
                    BodyData = new BodyData
                    {
                        DetectedBodyType = BodyType.String,
                        BodyAsString = request.Path + "Bar"
                    },
                    StatusCode = 302
                });

            // Act
            var response = await responseBuilder.ProvideResponseAsync(requestMessage, _settings).ConfigureAwait(false);

            // Assert
            response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
            response.Message.StatusCode.Should().Be(302);
        }

        [Fact]
        public async Task Response_WithCallback_ShouldUseStatusCodeAndHeaderInTheCallback()
        {
            // Assign
            var header = "X-UserId";
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var responseBuilder = Response.Create()
                .WithCallback(request => new ResponseMessage
                {
                    BodyData = new BodyData
                    {
                        DetectedBodyType = BodyType.String,
                        BodyAsString = request.Path + "Bar"
                    },
                    StatusCode = HttpStatusCode.Accepted,
                    Headers = new Dictionary<string, WireMockList<string>>
                    {
                        { header, new WireMockList<string>("Stef") }
                    }
                });

            // Act
            var response = await responseBuilder.ProvideResponseAsync(requestMessage, _settings).ConfigureAwait(false);

            // Assert
            response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
            response.Message.StatusCode.Should().Be(HttpStatusCode.Accepted);
            response.Message.Headers[header].Should().ContainSingle("Stef");
        }

        [Fact]
        public async Task Response_WithCallback_And_Additional_WithStatusCode_And_WithHeader_ShouldUseAdditional()
        {
            // Assign
            var header = "X-UserId";
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var responseBuilder = Response.Create()
                .WithCallback(request => new ResponseMessage
                {
                    BodyData = new BodyData
                    {
                        DetectedBodyType = BodyType.String,
                        BodyAsString = request.Path + "Bar"
                    },
                    StatusCode = HttpStatusCode.NotFound,
                    Headers = new Dictionary<string, WireMockList<string>>
                    {
                        { header, new WireMockList<string>("NA") }
                    }
                })
                .WithStatusCode(HttpStatusCode.Accepted)
                .WithHeader(header, "Stef");

            // Act
            var response = await responseBuilder.ProvideResponseAsync(requestMessage, _settings).ConfigureAwait(false);

            // Assert
            response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
            response.Message.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            response.Message.Headers[header].Should().ContainSingle("Stef");
        }

        [Fact]
        public async Task Response_WithCallback_And_UseTransformer_Is_True()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var responseBuilder = Response.Create()
                .WithCallback(request => new ResponseMessage
                {
                    BodyData = new BodyData
                    {
                        DetectedBodyType = BodyType.String,
                        BodyAsString = "{{request.Path}}Bar"
                    },
                    StatusCode = 302
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(requestMessage, _settings).ConfigureAwait(false);

            // Assert
            response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
            response.Message.StatusCode.Should().Be(302);
        }
    }
}