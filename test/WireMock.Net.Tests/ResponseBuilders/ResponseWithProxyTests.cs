using Moq;
using NFluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly Mock<IWireMockServerSettings> _settingsMock = new Mock<IWireMockServerSettings>();
        private readonly WireMockServer _server;
        private readonly Guid _guid;

        public ResponseWithProxyTests()
        {
            _guid = Guid.NewGuid();

            _server = WireMockServer.Start();
            _server.Given(Request.Create().UsingPost().WithPath($"/{_guid}"))
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 42 }).WithHeader("Content-Type", "application/json"));
        }

        [Fact]
        public async Task Response_WithProxy()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/xml" } } };
            var request = new RequestMessage(new UrlDetails($"{_server.Urls[0]}/{_guid}"), "POST", "::1", new BodyData { DetectedBodyType = BodyType.Json, BodyAsJson = new { a = 1 } }, headers);
            var response = Response.Create().WithProxy(_server.Urls[0]);

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).IsEqualTo("{\"p\":42}");
            Check.That(responseMessage.StatusCode).IsEqualTo(201);
            Check.That(responseMessage.Headers["Content-Type"].ToString()).IsEqualTo("application/json");
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}