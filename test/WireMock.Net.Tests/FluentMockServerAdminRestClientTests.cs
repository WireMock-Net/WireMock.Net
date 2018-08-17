using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerAdminRestClientTests : IDisposable
    {
        public void Dispose()
        {
            _server?.Stop();
        }

        private FluentMockServer _server;

        [Fact]
        public async Task IFluentMockServerAdmin_FindRequestsAsync()
        {
            // given
            _server = FluentMockServer.Start(new FluentMockServerSettings { StartAdminInterface = true, Logger = new WireMockNullLogger() });
            var serverUrl = "http://localhost:" + _server.Ports[0];
            await new HttpClient().GetAsync(serverUrl + "/foo");
            var api = RestClient.For<IFluentMockServerAdmin>(serverUrl);

            // when
            var requests = await api.FindRequestsAsync(new RequestModel { Methods = new[] { "get" } });

            // then
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("get");
            Check.That(requestLogged.Request.Body).IsNull();
            Check.That(requestLogged.Request.Path).IsEqualTo("/foo");
        }

        [Fact]
        public async Task IFluentMockServerAdmin_GetRequestsAsync()
        {
            // given
            _server = FluentMockServer.Start(new FluentMockServerSettings { StartAdminInterface = true, Logger = new WireMockNullLogger() });
            var serverUrl = "http://localhost:" + _server.Ports[0];
            await new HttpClient().GetAsync(serverUrl + "/foo");
            var api = RestClient.For<IFluentMockServerAdmin>(serverUrl);

            // when
            var requests = await api.GetRequestsAsync();

            // then
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("get");
            Check.That(requestLogged.Request.Body).IsNull();
            Check.That(requestLogged.Request.Path).IsEqualTo("/foo");
        }

        [Fact]
        public async Task IFluentMockServerAdmin_GetRequestsAsync_JsonApi()
        {
            // given
            _server = FluentMockServer.Start(new FluentMockServerSettings { StartAdminInterface = true, Logger = new WireMockNullLogger() });
            var serverUrl = "http://localhost:" + _server.Ports[0];
            var data = "{\"data\":[{\"type\":\"program\",\"attributes\":{\"alias\":\"T000001\",\"title\":\"Title Group Entity\"}}]}";
            var jsonApiAcceptHeader = "application/vnd.api+json";
            var jsonApiContentType = "application/vnd.api+json";
            var request = new HttpRequestMessage(HttpMethod.Post, serverUrl);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonApiAcceptHeader));
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(jsonApiContentType);
            var response = new HttpClient().SendAsync(request);

            var api = RestClient.For<IFluentMockServerAdmin>(serverUrl);

            // when
            var requests = await api.GetRequestsAsync();

            // then
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("post");
            Check.That(requestLogged.Request.Body).IsNotNull();
            Check.That(requestLogged.Request.Body).Contains("T000001");
        }

        [Fact]
        public async Task IFluentMockServerAdmin_GetRequestsAsync_Json()
        {
            // given
            _server = FluentMockServer.Start(new FluentMockServerSettings { StartAdminInterface = true, Logger = new WireMockNullLogger() });
            var serverUrl = "http://localhost:" + _server.Ports[0];
            var data = "{\"alias\": \"T000001\"}";
            var jsonAcceptHeader = "application/json";
            var jsonApiContentType = "application/json";
            var request = new HttpRequestMessage(HttpMethod.Post, serverUrl);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonAcceptHeader));
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(jsonApiContentType);
            var response = new HttpClient().SendAsync(request);

            var api = RestClient.For<IFluentMockServerAdmin>(serverUrl);

            // when
            var requests = await api.GetRequestsAsync();

            // then
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("post");
            Check.That(requestLogged.Request.Body).IsNotNull();
            Check.That(requestLogged.Request.Body).Contains("T000001");
        }
    }
}