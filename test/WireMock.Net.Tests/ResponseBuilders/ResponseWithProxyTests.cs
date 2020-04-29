﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithProxyTests : IDisposable
    {
        private const string ClientIp = "::1";
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();
        private readonly WireMockServer _server;
        private readonly Guid _guid;

        public ResponseWithProxyTests()
        {
            _guid = Guid.NewGuid();

            _server = WireMockServer.Start();
            _server.Given(Request.Create().UsingPost().WithPath($"/{_guid}"))
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 42 }).WithHeader("Content-Type", "application/json"));
            _server.Given(Request.Create().UsingPost().WithPath($"/{_guid}/append"))
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 10 }).WithHeader("Content-Type", "application/json"));
            _server.Given(Request.Create().UsingPost().WithPath($"/prepend/{_guid}"))
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 11 }).WithHeader("Content-Type", "application/json"));
            _server.Given(Request.Create().UsingPost().WithPath($"/prepend/{_guid}/append"))
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 12 }).WithHeader("Content-Type", "application/json"));
        }

        [Theory]
        [InlineData("", "", "{\"p\":42}")]
        [InlineData("", "/append", "{\"p\":10}")]
        [InlineData("/prepend", "", "{\"p\":11}")]
        [InlineData("/prepend", "/append", "{\"p\":12}")]
        public async Task Response_WithProxy(string prepend, string append, string expectedBody)
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/xml" } } };
            var request = new RequestMessage(new UrlDetails($"{_server.Urls[0]}{prepend}/{_guid}{append}"), "POST", ClientIp, new BodyData { DetectedBodyType = BodyType.Json, BodyAsJson = new { a = 1 } }, headers);
            var response = Response.Create().WithProxy(_server.Urls[0]);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(request.ProxyUrl).IsNotNull();
            Check.That(responseMessage.BodyData.BodyAsString).IsEqualTo(expectedBody);
            Check.That(responseMessage.StatusCode).IsEqualTo(201);
            Check.That(responseMessage.Headers["Content-Type"].ToString()).IsEqualTo("application/json");
        }

        [Fact]
        public void Response_WithProxy_WebProxySettings()
        {
            // Assign
            var settings = new ProxyAndRecordSettings
            {
                Url = "http://test.nl",
                WebProxySettings = new WebProxySettings
                {
                    Address = "http://company",
                    UserName = "x",
                    Password = "y"
                }
            };
            var response = Response.Create().WithProxy(settings);

            // Act
            var request = new RequestMessage(new UrlDetails($"{_server.Urls[0]}/{_guid}"), "GET", ClientIp);

            Check.ThatAsyncCode(() => response.ProvideResponseAsync(request, _settings)).Throws<HttpRequestException>();
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}