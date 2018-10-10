using NFluent;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;
using Xunit.Abstractions;

namespace WireMock.Net.Tests
{
    public class FluentMockServerProxy2Tests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly CancellationTokenSource _cts;
        private Guid _guid;
        private string _url;

        public FluentMockServerProxy2Tests(ITestOutputHelper output)
        {
            _output = output;

            _cts = new CancellationTokenSource();
        }

        //private Task Run()
        //{
            
        //    return Task.Run(() =>
        //    {
        //        _guid = Guid.NewGuid();

        //        var targetServer = FluentMockServer.Start();
        //        targetServer.Given(Request.Create().UsingPost().WithPath($"/{_guid}"))
        //            .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 42 }).WithHeader("Content-Type", "application/json"));

        //        _url = targetServer.Urls[0];

        //        //while (!_cts.IsCancellationRequested)
        //        //{
        //        //    Thread.Sleep(100);
        //        //}
        //    }, _cts.Token);
        //}

        private void X()
        {
            _guid = Guid.NewGuid();

            var targetServer = FluentMockServer.Start();
            targetServer.Given(Request.Create().UsingPost().WithPath($"/{_guid}"))
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 42 }).WithHeader("Content-Type", "application/json"));

            _url = targetServer.Urls[0];

            // Thread.Sleep(TimeSpan.FromSeconds(3));

            _output.WriteLine(targetServer.Urls[0]);

            //while (!_cts.IsCancellationRequested)
            //{
            //    Thread.Sleep(100);
            //}
        }

        [Fact]
        public void FluentMockServer_ProxyAndRecordSettings_ShouldProxyContentTypeHeader()
        {
            // Assign
            _output.WriteLine("This is output fr");
            //var t = new Thread(X);
            //t.Start();
            X();
            _output.WriteLine("started");

            Thread.Sleep(TimeSpan.FromSeconds(4));

            _output.WriteLine("sleep 4 done");

            var server = FluentMockServer.Start(
                new FluentMockServerSettings
                {
                    ProxyAndRecordSettings = new ProxyAndRecordSettings
                    {
                        Url = _url
                    }
                }
            );

            _output.WriteLine("started 2");
            _output.WriteLine(server.Urls[0]);

            // Act
            var response = new HttpClient().PostAsync(new Uri($"{server.Urls[0]}/{_guid}"), new StringContent("{ \"x\": 1 }", Encoding.UTF8, "application/json")).Result;
            //string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            //// Assert
            //Check.That(content).IsEqualTo("{\"p\":42}");
            //Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
            //Check.That(response.Content.Headers.GetValues("Content-Type").First()).IsEqualTo("application/json");
        }

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}