﻿using System;
using System.Linq;
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
    public class StatefulBehaviorTests
    {
        [Fact]
        public async Task Scenarios_Should_skip_non_relevant_states()
        {
            // given
            string path = $"/foo_{Guid.NewGuid()}";
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create().WithPath(path).UsingGet())
                .InScenario("s")
                .WhenStateIs("Test state")
                .RespondWith(Response.Create());

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + server.Ports[0] + path);

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Scenarios_Should_process_request_if_equals_state_and_single_state_defined()
        {
            // given
            string path = $"/foo_{Guid.NewGuid()}";
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create().WithPath(path).UsingGet())
                .InScenario("s")
                .WillSetStateTo("Test state")
                .RespondWith(Response.Create().WithBody("No state msg"));

            server
                .Given(Request.Create().WithPath(path).UsingGet())
                .InScenario("s")
                .WhenStateIs("Test state")
                .RespondWith(Response.Create().WithBody("Test state msg"));

            // when
            var responseNoState = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + path);
            var responseWithState = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + path);

            // then
            Check.That(responseNoState).Equals("No state msg");
            Check.That(responseWithState).Equals("Test state msg");
        }

        [Fact]
        public async Task Scenarios_TodoList_Example()
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

            Check.That(server.Scenarios.Any()).IsFalse();

            // Act and Assert
            string url = "http://localhost:" + server.Ports[0];
            string getResponse1 = new HttpClient().GetStringAsync(url + "/todo/items").Result;
            Check.That(getResponse1).Equals("Buy milk");

            Check.That(server.Scenarios["To do list"].Name).IsEqualTo("To do list");
            Check.That(server.Scenarios["To do list"].NextState).IsEqualTo("TodoList State Started");
            Check.That(server.Scenarios["To do list"].Started).IsTrue();
            Check.That(server.Scenarios["To do list"].Finished).IsFalse();

            var postResponse = await new HttpClient().PostAsync(url + "/todo/items", new StringContent("Cancel newspaper subscription"));
            Check.That(postResponse.StatusCode).Equals(HttpStatusCode.Created);

            Check.That(server.Scenarios["To do list"].Name).IsEqualTo("To do list");
            Check.That(server.Scenarios["To do list"].NextState).IsEqualTo("Cancel newspaper item added");
            Check.That(server.Scenarios["To do list"].Started).IsTrue();
            Check.That(server.Scenarios["To do list"].Finished).IsFalse();

            string getResponse2 = await new HttpClient().GetStringAsync(url + "/todo/items");
            Check.That(getResponse2).Equals("Buy milk;Cancel newspaper subscription");

            Check.That(server.Scenarios["To do list"].Name).IsEqualTo("To do list");
            Check.That(server.Scenarios["To do list"].NextState).IsNull();
            Check.That(server.Scenarios["To do list"].Started).IsTrue();
            Check.That(server.Scenarios["To do list"].Finished).IsTrue();
        }

        [Fact]
        public async Task Scenarios_Should_process_request_if_equals_state_and_multiple_state_defined()
        {
            // Assign
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create().WithPath("/state1").UsingGet())
                .InScenario("s1")
                .WillSetStateTo("Test state 1")
                .RespondWith(Response.Create().WithBody("No state msg 1"));

            server
                .Given(Request.Create().WithPath("/foo1X").UsingGet())
                .InScenario("s1")
                .WhenStateIs("Test state 1")
                .RespondWith(Response.Create().WithBody("Test state msg 1"));

            server
                .Given(Request.Create().WithPath("/state2").UsingGet())
                .InScenario("s2")
                .WillSetStateTo("Test state 2")
                .RespondWith(Response.Create().WithBody("No state msg 2"));

            server
                .Given(Request.Create().WithPath("/foo2X").UsingGet())
                .InScenario("s2")
                .WhenStateIs("Test state 2")
                .RespondWith(Response.Create().WithBody("Test state msg 2"));

            // Act and Assert
            string url = "http://localhost:" + server.Ports[0];
            var responseNoState1 = await new HttpClient().GetStringAsync(url + "/state1");
            Check.That(responseNoState1).Equals("No state msg 1");

            var responseNoState2 = await new HttpClient().GetStringAsync(url + "/state2");
            Check.That(responseNoState2).Equals("No state msg 2");

            var responseWithState1 = await new HttpClient().GetStringAsync(url + "/foo1X");
            Check.That(responseWithState1).Equals("Test state msg 1");

            var responseWithState2 = await new HttpClient().GetStringAsync(url + "/foo2X");
            Check.That(responseWithState2).Equals("Test state msg 2");
        }
    }
}