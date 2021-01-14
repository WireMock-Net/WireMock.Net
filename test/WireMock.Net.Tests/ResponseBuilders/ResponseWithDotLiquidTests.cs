﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;
using WireMock.Handlers;
using Moq;
#if NET452
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithDotLiquidTests
    {
        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();
        private const string ClientIp = "::1";

        public ResponseWithDotLiquidTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

        [Fact]
        public async Task Response_ProvideResponse_DotLiquid_WithNullBody_ShouldNotThrowException()
        {
            // Assign
            var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
            var request = new RequestMessage(urlDetails, "GET", ClientIp);

            var response = Response.Create().WithTransformer(TransformerType.DotLiquid);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            responseMessage.BodyData.Should().BeNull();
        }

        [Fact]
        public async Task Response_ProvideResponse_DotLiquid_UrlPathVerb()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "whatever",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POSt", ClientIp, body);

            var response = Response.Create()
                .WithBody("test {{request.url}} {{request.path}} {{request.method}}")
                .WithTransformer(TransformerType.DotLiquid);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("test http://localhost/foo /foo POSt");
        }
    }
}