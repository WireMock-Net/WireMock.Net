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
                .InScenario("s")
                .WhenStateIs("Test state")
                .RespondWith(Response.Create());

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_process_request_if_equals_state_and_single_state_defined()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .InScenario("s")
                .WillSetStateTo("Test state")
                .RespondWith(Response.Create()
                    .WithBody("No state msg"));

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .InScenario("s")
                .WhenStateIs("Test state")
                .RespondWith(Response.Create()
                    .WithBody("Test state msg"));

            // when
            var responseNoState = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            var responseWithState = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(responseNoState).Equals("No state msg");
            Check.That(responseWithState).Equals("Test state msg");
        }

        [Fact]
        public async Task Should_process_request_if_equals_state_and_multiple_state_defined()
        {
            // given
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/state1")
                    .UsingGet())
                .InScenario("s1")
                .WillSetStateTo("Test state 1")
                .RespondWith(Response.Create()
                    .WithBody("No state msg 1"));

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .InScenario("s1")
                .WhenStateIs("Test state 1")
                .RespondWith(Response.Create()
                    .WithBody("Test state msg 1"));

            _server
                .Given(Request.Create()
                    .WithPath("/state2")
                    .UsingGet())
                .InScenario("s2")
                .WillSetStateTo("Test state 2")
                .RespondWith(Response.Create()
                    .WithBody("No state msg 2"));

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .InScenario("s2")
                .WhenStateIs("Test state 2")
                .RespondWith(Response.Create()
                    .WithBody("Test state msg 2"));

            // when
            string url = "http://localhost:" + _server.Ports[0];
            var responseNoState1 = await new HttpClient().GetStringAsync(url + "/state1");
            var responseNoState2 = await new HttpClient().GetStringAsync(url + "/state2");

            var responseWithState1 = await new HttpClient().GetStringAsync(url + "/foo");
            var responseWithState2 = await new HttpClient().GetStringAsync(url + "/foo");

            // then
            Check.That(responseNoState1).Equals("No state msg 1");
            Check.That(responseWithState1).Equals("Test state msg 1");
            Check.That(responseNoState2).Equals("No state msg 2");
            Check.That(responseWithState2).Equals("Test state msg 2");
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}