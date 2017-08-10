#if NETSTANDARD
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WireMock.Http;
using WireMock.Validation;

namespace WireMock.Owin
{
    internal class AspNetCoreSelfHost : IOwinSelfHost
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly WireMockMiddlewareOptions _options;
        private readonly string[] _uriPrefixes;

        public bool IsStarted { get; private set; }

        public List<string> Urls { get; } = new List<string>();

        public List<int> Ports { get; } = new List<int>();

        public AspNetCoreSelfHost([NotNull] WireMockMiddlewareOptions options, [NotNull] params string[] uriPrefixes)
        {
            Check.NotNull(options, nameof(options));
            Check.NotEmpty(uriPrefixes, nameof(uriPrefixes));

            foreach (string uriPrefix in uriPrefixes)
            {
                Urls.Add(uriPrefix);

                int port;
                string host;
                PortUtil.TryExtractProtocolAndPort(uriPrefix, out host, out port);
                Ports.Add(port);
            }

            _options = options;
            _uriPrefixes = uriPrefixes;
        }

        public Task StartAsync()
        {
            IWebHost host = new WebHostBuilder()
                    // .ConfigureLogging(factory => factory.AddConsole(LogLevel.None))
                    .Configure(appBuilder =>
                    {
                        appBuilder.UseMiddleware<WireMockMiddleware>(_options);
                    })
                    .UseKestrel()
                    .UseUrls(_uriPrefixes)
                    .Build();

#if NETSTANDARD1_3
            System.Console.WriteLine("WireMock.Net server using netstandard1.3");
            return Task.Run(() =>
            {
                host.Run(_cts.Token);
                IsStarted = true;
            }, _cts.Token);
#else
            System.Console.WriteLine("WireMock.Net server using netstandard2.0");
            IsStarted = true;
            return host.RunAsync(_cts.Token);
#endif
        }

        public Task StopAsync()
        {
            _cts.Cancel();

            IsStarted = false;

            return Task.FromResult(true);
        }
    }
}
#endif