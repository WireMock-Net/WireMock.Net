using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class StatefulBehaviorTests : IDisposable
    {
        private FluentMockServer _server;

        [Fact]
        public async Task Should_skip_non_relevant_states()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .WhenStateIs("Test state")
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"{ msg: ""Hello world!""}"));

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_process_request_if_equals_state()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .WillSetStateTo("Test state")
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"No state msg"));

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .WhenStateIs("Test state")
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"Test state msg"));

            // when
            var responseNoState = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            var responseWithState = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(responseNoState).Equals("No state msg");
            Check.That(responseWithState).Equals("Test state msg");
        }

        [Fact]
        public async Task Should_not_change_state_if_next_state_is_not_defined()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .WillSetStateTo("Test state")
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"No state msg"));

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .WhenStateIs("Test state")
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"Test state msg"));

            // when
            var responseNoState = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            var responseWithState = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            var responseWithStateNotChanged = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(responseNoState).Equals("No state msg");
            Check.That(responseWithState).Equals("Test state msg");
            Check.That(responseWithStateNotChanged).Equals("Test state msg");
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
