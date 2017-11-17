#if NETSTANDARD
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using WireMock.Http;
using WireMock.Validation;

namespace WireMock.Owin
{
    internal class AspNetCoreSelfHost : IOwinSelfHost
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly WireMockMiddlewareOptions _options;
        private readonly string[] _uriPrefixes;

        private IWebHost _host;

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

                PortUtil.TryExtractProtocolAndPort(uriPrefix, out string host, out int port);
                Ports.Add(port);
            }

            _options = options;
            _uriPrefixes = uriPrefixes;
        }

        public Task StartAsync()
        {
            _host = new WebHostBuilder()
                .Configure(appBuilder =>
                {
                    appBuilder.UseMiddleware<GlobalExceptionMiddleware>();
                    _options.PreWireMockMiddlewareInit?.Invoke(appBuilder);
                    appBuilder.UseMiddleware<WireMockMiddleware>(_options);
                    _options.PostWireMockMiddlewareInit?.Invoke(appBuilder);
                })
                .UseKestrel()
                .UseUrls(_uriPrefixes)
                .Build();

#if NETSTANDARD1_3
            System.Console.WriteLine("WireMock.Net server using netstandard1.3");
            return Task.Run(() =>
            {
                _host.Run(_cts.Token);
                IsStarted = true;
            }, _cts.Token);
#else
            System.Console.WriteLine("WireMock.Net server using netstandard2.0");
            IsStarted = true;
            return _host.RunAsync(_cts.Token);
#endif
        }

        public Task StopAsync()
        {
            _cts.Cancel();

            IsStarted = false;
#if NETSTANDARD1_3
            return Task.FromResult(true);
#else
            return _host.WaitForShutdownAsync();
#endif
        }
    }
}
#endif