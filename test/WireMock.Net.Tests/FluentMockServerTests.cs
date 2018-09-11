using NFluent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerTests
    {
        private static string jsonRequestMessage = @"{ ""message"" : ""Hello server"" }";

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_methodPatch()
        {
            // given
            string path = $"/foo_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath(path).UsingMethod("patch"))
                .RespondWith(Response.Create().WithBody("hello patch"));

            // when
            var msg = new HttpRequestMessage(new HttpMethod("patch"), new Uri("http://localhost:" + _server.Ports[0] + path))
            {
                Content = new StringContent("{\"data\": {\"attr\":\"value\"}}")
            };
            var response = await new HttpClient().SendAsync(msg);

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            Check.That(responseBody).IsEqualTo("hello patch");

            Check.That(_server.LogEntries).HasSize(1);
            var requestLogged = _server.LogEntries.First();
            Check.That(requestLogged.RequestMessage.Method).IsEqualTo("patch");
            Check.That(requestLogged.RequestMessage.Body).IsNotNull();
            Check.That(requestLogged.RequestMessage.Body).IsEqualTo("{\"data\": {\"attr\":\"value\"}}");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_bodyAsString()
        {
            // given
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("Hello world!"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response).IsEqualTo("Hello world!");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_BodyAsJson()
        {
            // Assign
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create().UsingAnyMethod())
                .RespondWith(Response.Create().WithBodyAsJson(new { message = "Hello" }));

            // Act
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0]);

            // Assert
            Check.That(response).IsEqualTo("{\"message\":\"Hello\"}");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_BodyAsJson_Indented()
        {
            // Assign
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create().UsingAnyMethod())
                .RespondWith(Response.Create().WithBodyAsJson(new { message = "Hello" }, true));

            // Act
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0]);

            // Assert
            Check.That(response).IsEqualTo($"{{{Environment.NewLine}  \"message\": \"Hello\"{Environment.NewLine}}}");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_bodyAsCallback()
        {
            // Assign
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(500)
                    .WithHeader("H1", "X1")
                    .WithBody(req => $"path: {req.Path}"));

            // Act
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // Assert
            string content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo("path: /foo");
            Check.That((int) response.StatusCode).IsEqualTo(500);
            Check.That(response.Headers.GetValues("H1")).ContainsExactly("X1");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_bodyAsBase64()
        {
            // given
            var _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath("/foo").UsingGet()).RespondWith(Response.Create().WithBodyFromBase64("SGVsbG8gV29ybGQ/"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response).IsEqualTo("Hello World?");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_bodyAsBytes()
        {
            // given
            string path = $"/foo_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath(path).UsingGet()).RespondWith(Response.Create().WithBody(new byte[] { 48, 49 }));

            // when
            var responseAsString = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + path);
            var responseAsBytes = await new HttpClient().GetByteArrayAsync("http://localhost:" + _server.Ports[0] + path);

            // then
            Check.That(responseAsString).IsEqualTo("01");
            Check.That(responseAsBytes).ContainsExactly(new byte[] { 48, 49 });
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_valid_matchers_when_sent_json()
        {
            // Assign
            var validMatchersForHelloServerJsonMessage = new List<object[]>
            {
                new object[] { new WildcardMatcher("*Hello server*"), "application/json" },
                new object[] { new WildcardMatcher("*Hello server*"), "text/plain" },
                new object[] { new ExactMatcher(jsonRequestMessage), "application/json" },
                new object[] { new ExactMatcher(jsonRequestMessage), "text/plain" },
                new object[] { new RegexMatcher("Hello server"), "application/json" },
                new object[] { new RegexMatcher("Hello server"), "text/plain" },
                new object[] { new JsonPathMatcher("$..[?(@.message == 'Hello server')]"), "application/json" },
                new object[] { new JsonPathMatcher("$..[?(@.message == 'Hello server')]"), "text/plain" }
            };

            var _server = FluentMockServer.Start();

            foreach (var item in validMatchersForHelloServerJsonMessage)
            {
                string path = $"/foo_{Guid.NewGuid()}";
                _server
                    .Given(Request.Create().WithPath(path).WithBody((IMatcher)item[0]))
                    .RespondWith(Response.Create().WithBody("Hello client"));

                // Act
                var content = new StringContent(jsonRequestMessage, Encoding.UTF8, (string)item[1]);
                var response = await new HttpClient().PostAsync("http://localhost:" + _server.Ports[0] + path, content);

                // Assert
                var responseString = await response.Content.ReadAsStringAsync();
                Check.That(responseString).Equals("Hello client");

                _server.ResetMappings();
                _server.ResetLogEntries();
            }
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_404_for_unexpected_request()
        {
            // given
            string path = $"/foo{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + path);

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            Check.That((int)response.StatusCode).IsEqualTo(404);
        }

        [Fact]
        public async Task FluentMockServer_Should_find_a_request_satisfying_a_request_spec()
        {
            // Assign
            string path = $"/bar_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + path);

            // then
            var result = _server.FindLogEntries(Request.Create().WithPath(new RegexMatcher("^/b.*"))).ToList();
            Check.That(result).HasSize(1);

            var requestLogged = result.First();
            Check.That(requestLogged.RequestMessage.Path).IsEqualTo(path);
            Check.That(requestLogged.RequestMessage.Url).IsEqualTo("http://localhost:" + _server.Ports[0] + path);
        }

        [Fact]
        public async Task FluentMockServer_Should_reset_requestlogs()
        {
            // given
            var _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");
            _server.ResetLogEntries();

            // then
            Check.That(_server.LogEntries).IsEmpty();
        }

        [Fact]
        public void FluentMockServer_Should_reset_mappings()
        {
            // given
            string path = $"/foo_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath(path)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}"));

            // when
            _server.ResetMappings();

            // then
            Check.That(_server.Mappings).IsEmpty();
            Check.ThatAsyncCode(() => new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + path))
                .ThrowsAny();
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_a_redirect_without_body()
        {
            // Assign
            string path = $"/foo_{Guid.NewGuid()}";
            string pathToRedirect = $"/bar_{Guid.NewGuid()}";

            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath(path)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(307)
                    .WithHeader("Location", pathToRedirect));
            _server
                .Given(Request.Create()
                    .WithPath(pathToRedirect)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("REDIRECT SUCCESSFUL"));

            // Act
            var response = await new HttpClient().GetStringAsync($"http://localhost:{_server.Ports[0]}{path}");

            // Assert
            Check.That(response).IsEqualTo("REDIRECT SUCCESSFUL");
        }

        [Fact]
        public async Task FluentMockServer_Should_delay_responses_for_a_given_route()
        {
            // given
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/*"))
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}")
                    .WithDelay(TimeSpan.FromMilliseconds(200)));

            // when
            var watch = new Stopwatch();
            watch.Start();
            await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            watch.Stop();

            // then
            Check.That(watch.ElapsedMilliseconds).IsStrictlyGreaterThan(200);
        }

        [Fact]
        public async Task FluentMockServer_Should_delay_responses()
        {
            // given
            var _server = FluentMockServer.Start();
            _server.AddGlobalProcessingDelay(TimeSpan.FromMilliseconds(200));
            _server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithBody(@"{ msg: ""Hello world!""}"));

            // when
            var watch = new Stopwatch();
            watch.Start();
            await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            watch.Stop();

            // then
            Check.That(watch.ElapsedMilliseconds).IsStrictlyGreaterThan(200);
        }

        //Leaving commented as this requires an actual certificate with password, along with a service that expects a client certificate
        //[Fact]
        //public async Task Should_proxy_responses_with_client_certificate()
        //{
        //    // given
        //    var _server = FluentMockServer.Start();
        //    _server
        //        .Given(Request.Create().WithPath("/*"))
        //        .RespondWith(Response.Create().WithProxy("https://server-that-expects-a-client-certificate", @"\\yourclientcertificatecontainingprivatekey.pfx", "yourclientcertificatepassword"));

        //    // when
        //    var result = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/someurl?someQuery=someValue");

        //    // then
        //    Check.That(result).Contains("google");
        //}

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_callback()
        {
            // Assign
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create().WithPath("/foo").UsingGet())
                .RespondWith(Response.Create().WithCallback(req => new ResponseMessage { Body = req.Path + "Bar" }));

            // Act
            string response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // Assert
            Check.That(response).IsEqualTo("/fooBar");
        }

#if !NET452
        [Fact]
        public async Task FluentMockServer_Should_not_exclude_restrictedResponseHeader_for_ASPNETCORE()
        {
            // Assign
            string path = $"/foo_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create().WithPath(path).UsingGet())
                .RespondWith(Response.Create().WithHeader("Keep-Alive", "k").WithHeader("test", "t"));

            // Act
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + path);

            // Assert
            Check.That(response.Headers.Contains("test")).IsTrue();
            Check.That(response.Headers.Contains("Keep-Alive")).IsTrue();
        }
#endif
    }
}