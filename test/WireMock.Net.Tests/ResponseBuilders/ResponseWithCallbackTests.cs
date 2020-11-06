using System.Threading.Tasks;
using FluentAssertions;
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
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        [Fact]
        public async Task Response_WithCallbackAsync()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var response = Response.Create()
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
            var responseMessage = await response.ProvideResponseAsync(requestMessage, _settings);

            // Assert
            responseMessage.BodyData.BodyAsString.Should().Be("/fooBar");
            responseMessage.StatusCode.Should().Be(302);
        }

        [Fact]
        public async Task Response_WithCallback()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var response = Response.Create()
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
            var responseMessage = await response.ProvideResponseAsync(requestMessage, _settings);

            // Assert
            responseMessage.BodyData.BodyAsString.Should().Be("/fooBar");
            responseMessage.StatusCode.Should().Be(302);
        }

        [Fact]
        public async Task Response_WithCallback_And_UseTransformer_Is_True()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var response = Response.Create()
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
            var responseMessage = await response.ProvideResponseAsync(requestMessage, _settings);

            // Assert
            responseMessage.BodyData.BodyAsString.Should().Be("/fooBar");
            responseMessage.StatusCode.Should().Be(302);
        }
    }
}