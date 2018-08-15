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
    public class ObservableLogEntriesTest
    {
        [Fact]
        public async void FluentMockServer_LogEntriesChanged()
        {
            // Assign
            string path = $"/log_{Guid.NewGuid()}";
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create()
                    .WithPath(path)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}"));

            int count = 0;
            server.LogEntriesChanged += (sender, args) => count++;

            // Act
            await new HttpClient().GetAsync($"http://localhost:{server.Ports[0]}{path}");

            // Assert
            Check.That(count).Equals(1);
        }

        [Fact]
        public async Task FluentMockServer_LogEntriesChanged_Parallel()
        {
            int expectedCount = 10;

            // Assign
            string path = $"/log_p_{Guid.NewGuid()}";
            var server = FluentMockServer.Start();

            server
                .Given(Request.Create()
                    .WithPath(path)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithSuccess());

            int count = 0;
            server.LogEntriesChanged += (sender, args) => count++;

            var http = new HttpClient();

            // Act
            var listOfTasks = new List<Task<HttpResponseMessage>>();
            for (var i = 0; i < expectedCount; i++)
            {
                Thread.Sleep(10);
                listOfTasks.Add(http.GetAsync($"{server.Urls[0]}{path}"));
            }
            var responses = await Task.WhenAll(listOfTasks);
            var countResponsesWithStatusNotOk = responses.Count(r => r.StatusCode != HttpStatusCode.OK);

            // Assert
            Check.That(countResponsesWithStatusNotOk).Equals(0);
            Check.That(count).Equals(expectedCount);
        }
    }
}