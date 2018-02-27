﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NFluent;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class StatefulBehaviorTests
    {
        [Fact]
        public async Task Should_skip_non_relevant_states()
        {
            // given
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create().WithPath("/foo").UsingGet())
                .InScenario("s")
                .WhenStateIs("Test state")
                .RespondWith(Response.Create());

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + server.Ports[0] + "/foo");

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

            server.Dispose();
        }

        [Fact]
        public async Task Should_process_request_if_equals_state_and_single_state_defined()
        {
            // given
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create().WithPath("/foo").UsingGet())
                .InScenario("s")
                .WillSetStateTo("Test state")
                .RespondWith(Response.Create().WithBody("No state msg"));

            server
                .Given(Request.Create().WithPath("/foo").UsingGet())
                .InScenario("s")
                .WhenStateIs("Test state")
                .RespondWith(Response.Create().WithBody("Test state msg"));

            // when
            var responseNoState = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/foo");
            var responseWithState = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/foo");

            // then
            Check.That(responseNoState).Equals("No state msg");
            Check.That(responseWithState).Equals("Test state msg");

            server.Dispose();
        }

        [Fact]
        public async Task Scenario_and_State_TodoList_Example()
        {
            // Assign
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create().WithPath("/todo/items").UsingGet())
                .InScenario("To do list")
                .WillSetStateTo("TodoList State Started")
                .RespondWith(Response.Create().WithBody("Buy milk"));

            server
                .Given(Request.Create().WithPath("/todo/items").UsingPost())
                .InScenario("To do list")
                .WhenStateIs("TodoList State Started")
                .WillSetStateTo("Cancel newspaper item added")
                .RespondWith(Response.Create().WithStatusCode(201));

            server
                .Given(Request.Create().WithPath("/todo/items").UsingGet())
                .InScenario("To do list")
                .WhenStateIs("Cancel newspaper item added")
                .RespondWith(Response.Create().WithBody("Buy milk;Cancel newspaper subscription"));

            // Act and Assert
            string url = "http://localhost:" + server.Ports[0];
            string getResponse1 = new HttpClient().GetStringAsync(url + "/todo/items").Result;
            Check.That(getResponse1).Equals("Buy milk");

            var postResponse = await new HttpClient().PostAsync(url + "/todo/items", new StringContent("Cancel newspaper subscription"));
            Check.That(postResponse.StatusCode).Equals(HttpStatusCode.Created);

            string getResponse2 = await new HttpClient().GetStringAsync(url + "/todo/items");
            Check.That(getResponse2).Equals("Buy milk;Cancel newspaper subscription");

            server.Dispose();
        }

        // [Fact]
        public async Task Should_process_request_if_equals_state_and_multiple_state_defined()
        {
            // Assign
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create().WithPath("/state1").UsingGet())
                .InScenario("s1")
                .WillSetStateTo("Test state 1")
                .RespondWith(Response.Create().WithBody("No state msg 1"));

            server
                .Given(Request.Create().WithPath("/fooX").UsingGet())
                .InScenario("s1")
                .WhenStateIs("Test state 1")
                .RespondWith(Response.Create().WithBody("Test state msg 1"));

            server
                .Given(Request.Create().WithPath("/state2").UsingGet())
                .InScenario("s2")
                .WillSetStateTo("Test state 2")
                .RespondWith(Response.Create().WithBody("No state msg 2"));

            server
                .Given(Request.Create().WithPath("/fooX").UsingGet())
                .InScenario("s2")
                .WhenStateIs("Test state 2")
                .RespondWith(Response.Create().WithBody("Test state msg 2"));

            // Act and Assert
            string url = "http://localhost:" + server.Ports[0];
            var responseNoState1 = await new HttpClient().GetStringAsync(url + "/state1");
            Check.That(responseNoState1).Equals("No state msg 1");

            var responseNoState2 = await new HttpClient().GetStringAsync(url + "/state2");
            Check.That(responseNoState2).Equals("No state msg 2");

            var responseWithState1 = await new HttpClient().GetStringAsync(url + "/fooX");
            Check.That(responseWithState1).Equals("Test state msg 1");

            var responseWithState2 = await new HttpClient().GetStringAsync(url + "/fooX");
            Check.That(responseWithState2).Equals("Test state msg 2");

            server.Dispose();
        }
    }
}