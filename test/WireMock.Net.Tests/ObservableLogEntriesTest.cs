using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using RestEase;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class ObservableLogEntriesTest: IDisposable
    {
        private FluentMockServer _server;

        [Fact]
        public async void Test()
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

            var count = 0;
            _server.LogEntriesChanged += (sender, args) => count++;

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(count).Equals(1);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
