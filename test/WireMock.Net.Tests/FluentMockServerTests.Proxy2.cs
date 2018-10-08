using NFluent;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerProxy2Tests : IDisposable
    {
        private Guid _guid;
        private string _url;

        public FluentMockServerProxy2Tests()
        {
            _guid = Guid.NewGuid();

            var targetServer = FluentMockServer.Start();
            targetServer.Given(Request.Create().UsingPost().WithPath($"/{_guid}"))
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 42 }).WithHeader("Content-Type", "application/json"));

            _url = targetServer.Urls[0];
        }

        [Fact]
        public async Task FluentMockServer_ProxyAndRecordSettings_ShouldProxyContentTypeHeader()
        {
            // Assign
            var server = FluentMockServer.Start(
                new FluentMockServerSettings
                {
                    ProxyAndRecordSettings = new ProxyAndRecordSettings
                    {
                        Url = _url
                    }
                }
            );

            // Act
            var response = await new HttpClient().PostAsync(new Uri($"{server.Urls[0]}/{_guid}"), new StringContent("{ \"x\": 1 }", Encoding.UTF8, "application/json"));
            string content = await response.Content.ReadAsStringAsync();

            // Assert
            Check.That(content).IsEqualTo("{\"p\":42}");
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
            Check.That(response.Content.Headers.GetValues("Content-Type").First()).IsEqualTo("application/json");
        }

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}