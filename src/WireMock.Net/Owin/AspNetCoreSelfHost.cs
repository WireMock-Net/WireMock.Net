#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using WireMock.Http;
using WireMock.HttpsCertificate;
using WireMock.Logging;
using WireMock.Validation;

namespace WireMock.Owin
{
    internal class AspNetCoreSelfHost : IOwinSelfHost
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly WireMockMiddlewareOptions _options;
        private readonly string[] _urls;
        private readonly IWireMockLogger _logger;
        private Exception _runningException;

        private IWebHost _host;

        public bool IsStarted { get; private set; }

        public List<string> Urls { get; } = new List<string>();

        public List<int> Ports { get; } = new List<int>();

        public Exception RunningException => _runningException;

        public AspNetCoreSelfHost([NotNull] WireMockMiddlewareOptions options, [NotNull] params string[] uriPrefixes)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNullOrEmpty(uriPrefixes, nameof(uriPrefixes));

            _logger = options.Logger ?? new WireMockConsoleLogger();

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
                    appBuilder.UseMiddleware<GlobalExceptionMiddleware>(_options);

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
                        options.Listen(System.Net.IPAddress.Any, port);
                    }

                    foreach (string url in _urls.Where(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                    {
                        PortUtil.TryExtractProtocolAndPort(url, out string host, out int port);
                        options.Listen(System.Net.IPAddress.Any, port, listenOptions =>
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

            IsStarted = true;

            return Task.Run(() =>
            {
                StartServers();
            }, _cts.Token);
        }

        private void StartServers()
        {
            try
            {
                IsStarted = true;
#if NETSTANDARD1_3
                _logger.Info("WireMock.Net server using netstandard1.3");
                _host.Run(_cts.Token);
#else
                _logger.Info("WireMock.Net server using netstandard2.0");
                _host.Run();
#endif
            }
            catch (Exception e)
            {
                _runningException = e;
                _logger.Error(e.ToString());
            }
            finally
            {
                IsStarted = false;
            }
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