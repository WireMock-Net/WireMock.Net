using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NFluent;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class ObservableLogEntriesTest : IDisposable
    {
        private FluentMockServer _server;

        [Fact]
        public async void FluentMockServer_LogEntriesChanged()
        {
            // Assign
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}"));

            int count = 0;
            _server.LogEntriesChanged += (sender, args) => count++;

            // Act
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // Assert
            Check.That(count).Equals(1);
        }

        [Fact]
        public async Task FluentMockServer_LogEntriesChanged_Parallel()
        {
            int expectedCount = 10;

            // Assign
            _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithDelay(6)
                    .WithSuccess());

            int count = 0;
            _server.LogEntriesChanged += (sender, args) => count++;

            var http = new HttpClient();

            // Act
            var listOfTasks = new List<Task<HttpResponseMessage>>();
            for (var i = 0; i < expectedCount; i++)
            {
                Thread.Sleep(100);
                listOfTasks.Add(http.GetAsync($"{_server.Urls[0]}/foo"));
            }
            var responses = await Task.WhenAll(listOfTasks);
            var countResponsesWithStatusNotOk = responses.Count(r => r.StatusCode != HttpStatusCode.OK);

            // Assert
            Check.That(countResponsesWithStatusNotOk).Equals(0);
            Check.That(count).Equals(expectedCount);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}