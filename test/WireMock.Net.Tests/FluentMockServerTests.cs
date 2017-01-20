using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

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
    [Timeout(5000)]
    public class FluentMockServerTests
    {
        private FluentMockServer _server;

        [Test]
        public async Task Should_respond_to_request()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithUrl("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"{ msg: ""Hello world!""}"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Port + "/foo");

            // then
            Check.That(response).IsEqualTo(@"{ msg: ""Hello world!""}");
        }

        [Test]
        public async Task Should_respond_to_request_bodyAsBase64()
        {
            // given
            _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithUrl("/foo").UsingGet()).RespondWith(Response.Create().WithBodyAsBase64("SGVsbG8gV29ybGQ/"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Port + "/foo");

            // then
            Check.That(response).IsEqualTo("Hello World?");
        }

        [Test]
        public async Task Should_respond_404_for_unexpected_request()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Port + "/foo");

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            Check.That((int)response.StatusCode).IsEqualTo(404);
        }

        [Test]
        public async Task Should_record_requests_in_the_requestlogs()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Port + "/foo");

            // then
            Check.That(_server.RequestLogs).HasSize(1);
            var requestLogged = _server.RequestLogs.First();
            Check.That(requestLogged.Verb).IsEqualTo("get");
            Check.That(requestLogged.BodyAsBytes).IsNull();
        }

        [Test]
        public async Task Should_find_a_request_satisfying_a_request_spec()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Port + "/foo");
            await new HttpClient().GetAsync("http://localhost:" + _server.Port + "/bar");

            // then
            var result = _server.SearchLogsFor(Request.Create().WithUrl("/b.*")).ToList();
            Check.That(result).HasSize(1);

            var requestLogged = result.First();
            Check.That(requestLogged.Path).IsEqualTo("/bar");
            Check.That(requestLogged.Url).IsEqualTo("http://localhost:" + _server.Port + "/bar");
        }

        [Test]
        public async Task Should_reset_requestlogs()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Port + "/foo");
            _server.Reset();

            // then
            Check.That(_server.RequestLogs).IsEmpty();
        }

        [Test]
        public void Should_reset_routes()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithUrl("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}"));

            // when
            _server.Reset();

            // then
            Check.ThatAsyncCode(() => new HttpClient().GetStringAsync("http://localhost:" + _server.Port + "/foo"))
                .ThrowsAny();
        }

        [Test]
        public async Task Should_respond_a_redirect_without_body()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithUrl("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(307)
                    .WithHeader("Location", "/bar"));
            _server
                .Given(Request.Create()
                    .WithUrl("/bar")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("REDIRECT SUCCESSFUL"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Port + "/foo");

            // then
            Check.That(response).IsEqualTo("REDIRECT SUCCESSFUL");
        }

        [Test]
        public async Task Should_delay_responses_for_a_given_route()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithUrl("/*"))
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}")
                    .WithDelay(TimeSpan.FromMilliseconds(2000)));

            // when
            var watch = new Stopwatch();
            watch.Start();
            await new HttpClient().GetStringAsync("http://localhost:" + _server.Port + "/foo");
            watch.Stop();

            // then
            Check.That(watch.ElapsedMilliseconds).IsGreaterThan(2000);
        }

        [Test]
        public async Task Should_delay_responses()
        {
            // given
            _server = FluentMockServer.Start();
            _server.AddRequestProcessingDelay(TimeSpan.FromMilliseconds(2000));
            _server
                .Given(Request.Create()
                    .WithUrl("/*"))
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}"));

            // when
            var watch = new Stopwatch();
            watch.Start();
            await new HttpClient().GetStringAsync("http://localhost:" + _server.Port + "/foo");
            watch.Stop();

            // then
            Check.That(watch.ElapsedMilliseconds).IsGreaterThan(2000);
        }

        [TearDown]
        public void ShutdownServer()
        {
            _server.Stop();
        }
    }
}