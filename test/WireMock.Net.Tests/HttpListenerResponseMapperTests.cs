using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using WireMock.Http;

namespace WireMock.Net.Tests
{
    [TestFixture]
    public class HttpListenerResponseMapperTests
    {
        private TinyHttpServer _server;
        private Task<HttpResponseMessage> _responseMsgTask;

        [Test]
        public void Should_map_status_code_from_original_response()
        {
            // given
            var response = new ResponseMessage { StatusCode = 404 };
            var httpListenerResponse = CreateHttpListenerResponse();

            // when
            new HttpListenerResponseMapper().Map(response, httpListenerResponse);

            // then
            Check.That(httpListenerResponse.StatusCode).IsEqualTo(404);
        }

        [Test]
        public void Should_map_headers_from_original_response()
        {
            // given
            var response = new ResponseMessage();
            response.AddHeader("cache-control", "no-cache");
            var httpListenerResponse = CreateHttpListenerResponse();

            // when
            new HttpListenerResponseMapper().Map(response, httpListenerResponse);

            // then
            Check.That(httpListenerResponse.Headers).HasSize(1);
            Check.That(httpListenerResponse.Headers.Keys).Contains("cache-control");
            Check.That(httpListenerResponse.Headers.Get("cache-control")).Contains("no-cache");
        }

        [Test]
        public void Should_map_body_from_original_response()
        {
            // given
            var response = new ResponseMessage
            {
                Body = "Hello !!!"
            };

            var httpListenerResponse = CreateHttpListenerResponse();

            // when
            new HttpListenerResponseMapper().Map(response, httpListenerResponse);

            // then
            var responseMessage = ToResponseMessage(httpListenerResponse);
            Check.That(responseMessage).IsNotNull();

            var contentTask = responseMessage.Content.ReadAsStringAsync();
            Check.That(contentTask.Result).IsEqualTo("Hello !!!");
        }

        [TearDown]
        public void StopServer()
        {
            _server?.Stop();
        }

        /// <summary>
        /// Dirty HACK to get HttpListenerResponse instances
        /// </summary>
        /// <returns>
        /// The <see cref="HttpListenerResponse"/>.
        /// </returns>
        public HttpListenerResponse CreateHttpListenerResponse()
        {
            var port = PortUtil.FindFreeTcpPort();
            var urlPrefix = "http://localhost:" + port + "/";
            var responseReady = new AutoResetEvent(false);
            HttpListenerResponse response = null;
            _server = new TinyHttpServer(
                context =>
                    {
                        response = context.Response;
                        responseReady.Set();
                    }, urlPrefix);
            _server.Start();
            _responseMsgTask = new HttpClient().GetAsync(urlPrefix);
            responseReady.WaitOne();
            return response;
        }

        public HttpResponseMessage ToResponseMessage(HttpListenerResponse listenerResponse)
        {
            listenerResponse.Close();
            _responseMsgTask.Wait();
            return _responseMsgTask.Result;
        }
    }
}
