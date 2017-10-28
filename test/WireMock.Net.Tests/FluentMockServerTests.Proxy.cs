using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NFluent;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public partial class FluentMockServerTests
    {
        private FluentMockServer _serverForProxyForwarding;

        [Fact]
        public async Task FluentMockServer_Should_proxy_responses()
        {
            // given
            _server = FluentMockServer.Start();
            _server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithProxy("http://www.google.com"));

            // when
            var result = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/search?q=test");

            // then
            Check.That(result).Contains("google");
        }

        [Fact]
        public async Task FluentMockServer_Should_preserve_content_header_in_proxied_request()
        {
            // given
            _serverForProxyForwarding = FluentMockServer.Start();
            _serverForProxyForwarding
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create());

            _server = FluentMockServer.Start();
            _server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithProxy(_serverForProxyForwarding.Urls[0]));

            // when
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_server.Urls[0]),
                Content = new StringContent("stringContent")
            };
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            await new HttpClient().SendAsync(requestMessage);

            // then
            var receivedRequest = _serverForProxyForwarding.LogEntries.First().RequestMessage;
            Check.That(receivedRequest.Body).IsEqualTo("stringContent");
            Check.That(receivedRequest.Headers).ContainsKey("Content-Type");
            Check.That(receivedRequest.Headers["Content-Type"]).ContainsExactly("text/plain");
        }

        [Fact]
        public async Task FluentMockServer_Should_preserve_content_header_in_proxied_response()
        {
            // given
            _serverForProxyForwarding = FluentMockServer.Start();
            _serverForProxyForwarding
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create()
                    .WithBody("body")
                    .WithHeader("Content-Type", "text/plain"));

            _server = FluentMockServer.Start();
            _server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithProxy(_serverForProxyForwarding.Urls[0]));

            // when
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_server.Urls[0])
            };
            var response = await new HttpClient().SendAsync(requestMessage);

            // then
            Check.That(await response.Content.ReadAsStringAsync()).IsEqualTo("body");
            Check.That(response.Content.Headers.Contains("Content-Type")).IsTrue();
            Check.That(response.Content.Headers.GetValues("Content-Type")).ContainsExactly("text/plain");
        }

        [Fact]
        public async Task FluentMockServer_Should_change_absolute_location_header_in_proxied_response()
        {
            // given
            _serverForProxyForwarding = FluentMockServer.Start();
            _serverForProxyForwarding
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.Redirect)
                    .WithHeader("Location", _serverForProxyForwarding.Urls[0] + "testpath"));

            _server = FluentMockServer.Start();
            _server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithProxy(_serverForProxyForwarding.Urls[0]));

            // when
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_server.Urls[0])
            };
            var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
            var response = await new HttpClient(httpClientHandler).SendAsync(requestMessage);

            // then
            Check.That(response.Headers.Contains("Location")).IsTrue();
            Check.That(response.Headers.GetValues("Location")).ContainsExactly(_server.Urls[0] + "testpath");
        }

        [Fact]
        public async Task FluentMockServer_Should_preserve_cookie_header_in_proxied_request()
        {
            // given
            _serverForProxyForwarding = FluentMockServer.Start();
            _serverForProxyForwarding
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create());

            _server = FluentMockServer.Start();
            _server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithProxy(_serverForProxyForwarding.Urls[0]));

            // when
            var requestUri = new Uri(_server.Urls[0]);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = requestUri
            };
            var clientHandler = new HttpClientHandler();
            clientHandler.CookieContainer.Add(requestUri, new Cookie("name", "value"));
            await new HttpClient(clientHandler).SendAsync(requestMessage);

            // then
            var receivedRequest = _serverForProxyForwarding.LogEntries.First().RequestMessage;
            Check.That(receivedRequest.Cookies).IsNotNull();
            Check.That(receivedRequest.Cookies).ContainsPair("name", "value");
        }
    }
}