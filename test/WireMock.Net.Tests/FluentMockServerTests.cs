using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests
{
    [TestFixture]
    [Timeout(5000)]
    public class FluentMockServerTests
    {
        private FluentMockServer _server;

        [Test]
        public void FluentMockServer_Admin_Mappings_Get()
        {
            var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
            _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath("/foo1").UsingGet())
                .WithGuid(guid)
                .RespondWith(Response.Create().WithStatusCode(201).WithBody("1"));

            _server.Given(Request.Create().WithPath("/foo2").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(202).WithBody("2"));

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(2);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(guid);

            Check.That(mappings[1].Guid).Not.Equals(guid);
        }

        [Test]
        public void FluentMockServer_Admin_Mappings_Add_SameGuid()
        {
            var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
            _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath("/1").UsingGet())
                .WithGuid(guid)
                .RespondWith(Response.Create().WithStatusCode(500));

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);
            Check.That(mappings.First().Guid).Equals(guid);

            _server.Given(Request.Create().WithPath("/2").UsingGet())
                .WithGuid(guid)
                .RespondWith(Response.Create().WithStatusCode(500));

            Check.That(mappings).HasSize(1);
            Check.That(mappings.First().Guid).Equals(guid);
        }

        [Test]
        public async Task FluentMockServer_Admin_Mappings_AtPriority()
        {
            _server = FluentMockServer.Start();

            // given
            _server.Given(Request.Create().WithPath("/1").UsingGet())
                .AtPriority(2)
                .RespondWith(Response.Create().WithStatusCode(200));

            _server.Given(Request.Create().WithPath("/1").UsingGet())
                .AtPriority(1)
                .RespondWith(Response.Create().WithStatusCode(400));

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(2);
            Check.That(mappings[0].Priority).Equals(2);
            Check.That(mappings[1].Priority).Equals(1);

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/1");

            // then
            Check.That((int)response.StatusCode).IsEqualTo(400);
        }

        [Test]
        public async Task FluentMockServer_Admin_Requests_Get()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(_server.LogEntries).HasSize(1);
            var requestLogged = _server.LogEntries.First();
            Check.That(requestLogged.RequestMessage.Method).IsEqualTo("get");
            Check.That(requestLogged.RequestMessage.BodyAsBytes).IsNull();
        }

        [Test]
        public async Task Should_respond_to_request()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"{ msg: ""Hello world!""}"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response).IsEqualTo(@"{ msg: ""Hello world!""}");
        }

        [Test]
        public async Task Should_respond_to_request_bodyAsBase64()
        {
            // given
            _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath("/foo").UsingGet()).RespondWith(Response.Create().WithBodyAsBase64("SGVsbG8gV29ybGQ/"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response).IsEqualTo("Hello World?");
        }

        [Test]
        public async Task Should_respond_404_for_unexpected_request()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            Check.That((int)response.StatusCode).IsEqualTo(404);
        }

        [Test]
        public async Task Should_find_a_request_satisfying_a_request_spec()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/bar");

            // then
            var result = _server.SearchLogsFor(Request.Create().WithPath(new RegexMatcher("^/b.*"))).ToList();
            Check.That(result).HasSize(1);

            var requestLogged = result.First();
            Check.That(requestLogged.RequestMessage.Path).IsEqualTo("/bar");
            Check.That(requestLogged.RequestMessage.Url).IsEqualTo("http://localhost:" + _server.Ports[0] + "/bar");
        }

        [Test]
        public async Task Should_reset_requestlogs()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");
            _server.ResetLogEntries();

            // then
            Check.That(_server.LogEntries).IsEmpty();
        }

        [Test]
        public void Should_reset_mappings()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}"));

            // when
            _server.ResetMappings();

            // then
            Check.That(_server.Mappings).IsEmpty();
            Check.ThatAsyncCode(() => new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo"))
                .ThrowsAny();
        }

        [Test]
        public async Task Should_respond_a_redirect_without_body()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(307)
                    .WithHeader("Location", "/bar"));
            _server
                .Given(Request.Create()
                    .WithPath("/bar")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("REDIRECT SUCCESSFUL"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

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
            Check.That(watch.ElapsedMilliseconds).IsGreaterThan(200);
        }

        [Test]
        public async Task Should_delay_responses()
        {
            // given
            _server = FluentMockServer.Start();
            _server.AddRequestProcessingDelay(TimeSpan.FromMilliseconds(200));
            _server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithBody(@"{ msg: ""Hello world!""}"));

            // when
            var watch = new Stopwatch();
            watch.Start();
            await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            watch.Stop();

            // then
            Check.That(watch.ElapsedMilliseconds).IsGreaterThan(200);
        }

        [TearDown]
        public void ShutdownServer()
        {
            _server.Stop();
        }
    }
}