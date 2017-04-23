//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using NFluent;
//using Xunit;
//using WireMock.Matchers;
//using WireMock.RequestBuilders;
//using WireMock.ResponseBuilders;
//using WireMock.Server;

//namespace WireMock.Net.Tests
//{
//    //[TestFixture]
//    public class HttpListenerRequestMapperTests : IDisposable
//    {
//        private FluentMockServer _server;

//        public HttpListenerRequestMapperTests()
//        {
//            _server = FluentMockServer.Start();
//        }

//        [Fact]
//        public async Task Should_map_uri_from_listener_request()
//        {
//            // given
//            var client = new HttpClient();

//            // when 
//            await client.GetAsync(MapperServer.UrlPrefix + "toto");

//            // then
//            Check.That(MapperServer.LastRequestMessage).IsNotNull();
//            Check.That(MapperServer.LastRequestMessage.Path).IsEqualTo("/toto");
//        }

//        [Fact]
//        public async Task Should_map_verb_from_listener_request()
//        {
//            // given
//            var client = new HttpClient();

//            // when 
//            await client.PutAsync(MapperServer.UrlPrefix, new StringContent("Hello!"));

//            // then
//            Check.That(MapperServer.LastRequestMessage).IsNotNull();
//            Check.That(MapperServer.LastRequestMessage.Method).IsEqualTo("put");
//        }

//        [Fact]
//        public async Task Should_map_body_from_listener_request()
//        {
//            // given
//            var client = new HttpClient();

//            // when 
//            await client.PutAsync(MapperServer.UrlPrefix, new StringContent("Hello!"));

//            // then
//            Check.That(MapperServer.LastRequestMessage).IsNotNull();
//            Check.That(MapperServer.LastRequestMessage.Body).IsEqualTo("Hello!");
//        }

//        [Fact]
//        public async Task Should_map_headers_from_listener_request()
//        {
//            // given
//            var client = new HttpClient();
//            client.DefaultRequestHeaders.Add("X-Alex", "1706");

//            // when 
//            await client.GetAsync(MapperServer.UrlPrefix);

//            // then
//            Check.That(MapperServer.LastRequestMessage).IsNotNull();
//            Check.That(MapperServer.LastRequestMessage.Headers).Not.IsNullOrEmpty();
//            Check.That(MapperServer.LastRequestMessage.Headers.Contains(new KeyValuePair<string, string>("X-Alex", "1706"))).IsTrue();
//        }

//        [Fact]
//        public async Task Should_map_params_from_listener_request()
//        {
//            // given
//            var client = new HttpClient();

//            // when 
//            await client.GetAsync(MapperServer.UrlPrefix + "index.html?id=toto");

//            // then
//            Check.That(MapperServer.LastRequestMessage).IsNotNull();
//            Check.That(MapperServer.LastRequestMessage.Path).EndsWith("/index.html");
//            Check.That(MapperServer.LastRequestMessage.GetParameter("id")).HasSize(1);
//        }

//        public void Dispose()
//        {
//            _server.Stop().Wait();
//        }
//    }
//}
