using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using WireMock.Http;

namespace WireMock.Net.Tests
{
    [TestFixture]
    public class HttpListenerRequestMapperTests
    {
        private MapperServer _server;

        [SetUp]
        public void StartListenerServer()
        {
            _server = MapperServer.Start();
        }

        [Test]
        public async Task Should_map_uri_from_listener_request()
        {
            // given
            var client = new HttpClient();

            // when 
            await client.GetAsync(MapperServer.UrlPrefix + "toto");

            // then
            Check.That(MapperServer.LastRequestMessage).IsNotNull();
            Check.That(MapperServer.LastRequestMessage.Path).IsEqualTo("/toto");
        }

        [Test]
        public async Task Should_map_verb_from_listener_request()
        {
            // given
            var client = new HttpClient();

            // when 
            await client.PutAsync(MapperServer.UrlPrefix, new StringContent("Hello!"));

            // then
            Check.That(MapperServer.LastRequestMessage).IsNotNull();
            Check.That(MapperServer.LastRequestMessage.Method).IsEqualTo("put");
        }

        [Test]
        public async Task Should_map_body_from_listener_request()
        {
            // given
            var client = new HttpClient();

            // when 
            await client.PutAsync(MapperServer.UrlPrefix, new StringContent("Hello!"));

            // then
            Check.That(MapperServer.LastRequestMessage).IsNotNull();
            Check.That(MapperServer.LastRequestMessage.Body).IsEqualTo("Hello!");
        }

        [Test]
        public async Task Should_map_headers_from_listener_request()
        {
            // given
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Alex", "1706");

            // when 
            await client.GetAsync(MapperServer.UrlPrefix);

            // then
            Check.That(MapperServer.LastRequestMessage).IsNotNull();
            Check.That(MapperServer.LastRequestMessage.Headers).Not.IsNullOrEmpty();
            Check.That(MapperServer.LastRequestMessage.Headers.Contains(new KeyValuePair<string, string>("X-Alex", "1706"))).IsTrue();
        }

        [Test]
        public async Task Should_map_params_from_listener_request()
        {
            // given
            var client = new HttpClient();

            // when 
            await client.GetAsync(MapperServer.UrlPrefix + "index.html?id=toto");

            // then
            Check.That(MapperServer.LastRequestMessage).IsNotNull();
            Check.That(MapperServer.LastRequestMessage.Path).EndsWith("/index.html");
            Check.That(MapperServer.LastRequestMessage.GetParameter("id")).HasSize(1);
        }

        [TearDown]
        public void StopListenerServer()
        {
            _server.Stop();
        }

        private class MapperServer : TinyHttpServer
        {
            private static volatile RequestMessage _lastRequestMessage;

            private MapperServer(Action<HttpListenerContext> httpHandler, string urlPrefix) : base(httpHandler, urlPrefix)
            {
            }

            public static RequestMessage LastRequestMessage
            {
                get
                {
                    return _lastRequestMessage;
                }

                private set
                {
                    _lastRequestMessage = value;
                }
            }

            public static string UrlPrefix { get; private set; }

            public new static MapperServer Start()
            {
                int port = PortUtil.FindFreeTcpPort();
                UrlPrefix = "http://localhost:" + port + "/";
                var server = new MapperServer(
                    context =>
                        {
                            LastRequestMessage = new HttpListenerRequestMapper().Map(context.Request);
                            context.Response.Close();
                        }, UrlPrefix);
                ((TinyHttpServer)server).Start();

                return server;
            }

            public new void Stop()
            {
                base.Stop();
                LastRequestMessage = null;
            }
        }
    }
}
