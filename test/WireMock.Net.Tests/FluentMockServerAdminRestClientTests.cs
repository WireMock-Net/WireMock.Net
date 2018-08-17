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
    public class FluentMockServerAdminRestClientTests
    {
        [Fact]
        public async Task IFluentMockServerAdmin_FindRequestsAsync()
        {
            // given
            var server = FluentMockServer.Start(new FluentMockServerSettings { StartAdminInterface = true, Logger = new WireMockNullLogger() });
            var serverUrl = "http://localhost:" + server.Ports[0];
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
            var server = FluentMockServer.Start(new FluentMockServerSettings { StartAdminInterface = true, Logger = new WireMockNullLogger() });
            var serverUrl = "http://localhost:" + server.Ports[0];
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
    }
}