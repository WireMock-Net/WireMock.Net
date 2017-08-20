using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using Xunit;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests
{
    //[TestFixture]
    //[Timeout(5000)]
    public class FluentMockServerTests : IDisposable
    {
        private FluentMockServer _server;

        // For for AppVeyor + OpenCover
        private string GetCurrentFolder()
        {
            string current = Directory.GetCurrentDirectory();
            //if (!current.EndsWith("WireMock.Net.Tests"))
            //    return Path.Combine(current, "test", "WireMock.Net.Tests");

            return current;
        }

        [Fact]
        public void FluentMockServer_StartStop()
        {
            var server1 = FluentMockServer.Start("http://localhost:9091/");
            server1.Stop();

            var server2 = FluentMockServer.Start("http://localhost:9091/");
            server2.Stop();
        }

        [Fact]
        public void FluentMockServer_ReadStaticMapping_WithNonGuidFilename()
        {
            var guid = Guid.Parse("04ee4872-9efd-4770-90d3-88d445265d0d");
            string title = "documentdb_root_title";

            _server = FluentMockServer.Start();

            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings", "documentdb_root.json");
            _server.ReadStaticMapping(folder);

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(guid);
            Check.That(mappings.First().Title).Equals(title);
        }

        [Fact]
        public void FluentMockServer_ReadStaticMapping_WithGuidFilename()
        {
            string guid = "00000002-ee28-4f29-ae63-1ac9b0802d86";

            _server = FluentMockServer.Start();
            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings", guid + ".json");
            _server.ReadStaticMapping(folder);

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(Guid.Parse(guid));
            Check.That(mappings.First().Title).IsNullOrEmpty();
        }

        [Fact]
        public void FluentMockServer_ReadStaticMappings()
        {
            _server = FluentMockServer.Start();

            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings");
            _server.ReadStaticMappings(folder);

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(2);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
            Check.That(requestLogged.RequestMessage.BodyAsBytes).IsEmpty();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public async Task Should_find_a_request_satisfying_a_request_spec()
        {
            // given
            _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/bar");

            // then
            var result = _server.FindLogEntries(Request.Create().WithPath(new RegexMatcher("^/b.*"))).ToList();
            Check.That(result).HasSize(1);

            var requestLogged = result.First();
            Check.That(requestLogged.RequestMessage.Path).IsEqualTo("/bar");
            Check.That(requestLogged.RequestMessage.Url).IsEqualTo("http://localhost:" + _server.Ports[0] + "/bar");
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
            Check.That(watch.ElapsedMilliseconds).IsStrictlyGreaterThan(200);
        }

        [Fact]
        public async Task Should_delay_responses()
        {
            // given
            _server = FluentMockServer.Start();
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

        [Fact]
        public async Task Should_proxy_responses()
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

        //Leaving commented as this requires an actual certificate with password, along with a service that expects a client certificate
        //[Fact]
        //public async Task Should_proxy_responses_with_client_certificate()
        //{
        //    // given
        //    _server = FluentMockServer.Start();
        //    _server
        //        .Given(Request.Create().WithPath("/*"))
        //        .RespondWith(Response.Create().WithProxy("https://server-that-expects-a-client-certificate", @"\\yourclientcertificatecontainingprivatekey.pfx", "yourclientcertificatepassword"));

        //    // when
        //    var result = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/someurl?someQuery=someValue");

        //    // then
        //    Check.That(result).Contains("google");
        //}

        [Fact]
        public async Task FluentMockServer_Logging_SetMaxRequestLogCount()
        {
            // Assign
            var client = new HttpClient();
            // Act
            _server = FluentMockServer.Start();
            _server.SetMaxRequestLogCount(2);

            await client.GetAsync("http://localhost:" + _server.Ports[0] + "/foo1");
            await client.GetAsync("http://localhost:" + _server.Ports[0] + "/foo2");
            await client.GetAsync("http://localhost:" + _server.Ports[0] + "/foo3");
            
            // Assert
            Check.That(_server.LogEntries).HasSize(2);
            
            var requestLoggedA = _server.LogEntries.First();
            Check.That(requestLoggedA.RequestMessage.Path).EndsWith("/foo2");

            var requestLoggedB = _server.LogEntries.Last();
            Check.That(requestLoggedB.RequestMessage.Path).EndsWith("/foo3");
        }

        public void Dispose()
        {
            _server?.Stop();
        }
    }
}