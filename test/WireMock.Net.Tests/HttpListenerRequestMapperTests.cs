using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using WireMock.Http;

[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules", 
        "SA1101:PrefixLocalCallsWithThis", 
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.NamingRules", 
        "SA1309:FieldNamesMustNotBeginWithUnderscore", 
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules", 
        "SA1600:ElementsMustBeDocumented", 
        Justification = "Reviewed. Suppression is OK here, as it's a tests class.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules", 
        "SA1633:FileMustHaveHeader", 
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
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
            Check.That(MapperServer.LastRequest).IsNotNull();
            Check.That(MapperServer.LastRequest.Url).IsEqualTo("/toto");
        }

        [Test]
        public async Task Should_map_verb_from_listener_request()
        {
            // given
            var client = new HttpClient();

            // when 
            await client.PutAsync(MapperServer.UrlPrefix, new StringContent("Hello!"));

            // then
            Check.That(MapperServer.LastRequest).IsNotNull();
            Check.That(MapperServer.LastRequest.Verb).IsEqualTo("put");
        }

        [Test]
        public async Task Should_map_body_from_listener_request()
        {
            // given
            var client = new HttpClient();

            // when 
            await client.PutAsync(MapperServer.UrlPrefix, new StringContent("Hello!"));

            // then
            Check.That(MapperServer.LastRequest).IsNotNull();
            Check.That(MapperServer.LastRequest.Body).IsEqualTo("Hello!");
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
            Check.That(MapperServer.LastRequest).IsNotNull();
            Check.That(MapperServer.LastRequest.Headers).Not.IsNullOrEmpty();
            Check.That(MapperServer.LastRequest.Headers.Contains(new KeyValuePair<string, string>("x-alex", "1706"))).IsTrue();
        }

        [Test]
        public async Task Should_map_params_from_listener_request()
        {
            // given
            var client = new HttpClient();

            // when 
            await client.GetAsync(MapperServer.UrlPrefix + "index.html?id=toto");

            // then
            Check.That(MapperServer.LastRequest).IsNotNull();
            Check.That(MapperServer.LastRequest.Path).EndsWith("/index.html");
            Check.That(MapperServer.LastRequest.GetParameter("id")).HasSize(1);
        }

        [TearDown]
        public void StopListenerServer()
        {
            _server.Stop();
        }

        private class MapperServer : TinyHttpServer
        {
            private static volatile Request _lastRequest;

            private MapperServer(string urlPrefix, Action<HttpListenerContext> httpHandler) : base(urlPrefix, httpHandler)
            {
            }

            public static Request LastRequest
            {
                get
                {
                    return _lastRequest;
                }

                private set
                {
                    _lastRequest = value;
                }
            }

            public static string UrlPrefix { get; private set; }

            public static new MapperServer Start()
            {
                var port = Ports.FindFreeTcpPort();
                UrlPrefix = "http://localhost:" + port + "/";
                var server = new MapperServer(
                    UrlPrefix, 
                    context =>
                        {
                            LastRequest = new HttpListenerRequestMapper().Map(context.Request);
                            context.Response.Close();
                        });
                ((TinyHttpServer)server).Start();
                return server;
            }

            public new void Stop()
            {
                base.Stop();
                LastRequest = null;
            }
        }
    }
}
