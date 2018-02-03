#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using WireMock.Http;
using WireMock.HttpsCertificate;
using WireMock.Validation;

namespace WireMock.Owin
{
    internal class AspNetCoreSelfHost : IOwinSelfHost
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly WireMockMiddlewareOptions _options;
        private readonly string[] _urls;

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
            _urls = uriPrefixes;
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
                .UseKestrel(options =>
                {
#if NETSTANDARD1_3
                    if (_urls.Any(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                    {
                        options.UseHttps(PublicCertificateHelper.GetX509Certificate2());
                    }
#else
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?tabs=aspnetcore2x
                    foreach (string url in _urls.Where(u => u.StartsWith("http://", StringComparison.OrdinalIgnoreCase)))
                    {
                        PortUtil.TryExtractProtocolAndPort(url, out string host, out int port);
                        options.Listen(IPAddress.Loopback, port);
                    }

                    foreach (string url in _urls.Where(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                    {
                        PortUtil.TryExtractProtocolAndPort(url, out string host, out int port);
                        options.Listen(IPAddress.Loopback, port, listenOptions =>
                        {
                            listenOptions.UseHttps(PublicCertificateHelper.GetX509Certificate2());
                        });
                    }
#endif
                })
#if NETSTANDARD1_3
                .UseUrls(_urls)
#endif
                .Build();

#if NETSTANDARD1_3
            Console.WriteLine("WireMock.Net server using netstandard1.3");
            return Task.Run(() =>
            {
                _host.Run(_cts.Token);
                IsStarted = true;
            }, _cts.Token);
#else
            System.Console.WriteLine("WireMock.Net server using netstandard2.0");
            IsStarted = true;
            return Task.Run(() =>
            {
                _host.Run();
                IsStarted = true;
            }, _cts.Token);
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