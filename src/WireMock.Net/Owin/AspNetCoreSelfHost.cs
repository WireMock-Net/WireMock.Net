#if USE_ASPNETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WireMock.HttpsCertificate;
using WireMock.Logging;
using WireMock.Owin.Mappers;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Owin
{
    internal class AspNetCoreSelfHost : IOwinSelfHost
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly IWireMockMiddlewareOptions _options;
        private readonly string[] _urls;
        private readonly IWireMockLogger _logger;
        private Exception _runningException;

        private IWebHost _host;

        public bool IsStarted { get; private set; }

        public List<string> Urls { get; } = new List<string>();

        public List<int> Ports { get; } = new List<int>();

        public Exception RunningException => _runningException;

        public AspNetCoreSelfHost([NotNull] IWireMockMiddlewareOptions options, [NotNull] params string[] uriPrefixes)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNullOrEmpty(uriPrefixes, nameof(uriPrefixes));

            _logger = options.Logger ?? new WireMockConsoleLogger();

            foreach (string uriPrefix in uriPrefixes)
            {
                Urls.Add(uriPrefix);

                PortUtils.TryExtract(uriPrefix, out string protocol, out string host, out int port);
                Ports.Add(port);
            }

            _options = options;
            _urls = uriPrefixes;
        }

        public Task StartAsync()
        {
            var builder = new WebHostBuilder();

            // Workaround for https://github.com/WireMock-Net/WireMock.Net/issues/292
            // On some platforms, AppContext.BaseDirectory is null, which causes WebHostBuilder to fail if ContentRoot is not
            // specified (even though we don't actually use that base path mechanism, since we have our own way of configuring
            // a filesystem handler).
            if (string.IsNullOrEmpty(AppContext.BaseDirectory))
            {
                builder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
            }

            _host = builder
                .ConfigureServices(services =>
                {
                    services.AddSingleton(_options);
                    services.AddSingleton<IMappingMatcher, MappingMatcher>();
                    services.AddSingleton<IOwinRequestMapper, OwinRequestMapper>();
                    services.AddSingleton<IOwinResponseMapper, OwinResponseMapper>();

                    services.AddRequestDecompression(o => 
                        {
                            o.Providers.Add<DeflateDecompressionProvider>();
                            o.Providers.Add<GzipDecompressionProvider>();
                        });
                    services.AddResponseCompression();
                })
                .Configure(appBuilder =>
                {
                    appBuilder.UseMiddleware<GlobalExceptionMiddleware>();

                    _options.PreWireMockMiddlewareInit?.Invoke(appBuilder);

                    appBuilder.UseMiddleware<WireMockMiddleware>();

                    _options.PostWireMockMiddlewareInit?.Invoke(appBuilder);

                    appBuilder.UseRequestDecompression();
                    appBuilder.UseResponseCompression();
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
                        PortUtils.TryExtract(url, out string protocol, out string host, out int port);
                        options.Listen(System.Net.IPAddress.Any, port);
                    }

                    foreach (string url in _urls.Where(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                    {
                        PortUtils.TryExtract(url, out string protocol, out string host, out int port);
                        options.Listen(System.Net.IPAddress.Any, port, listenOptions =>
                        {
                            listenOptions.UseHttps(); // PublicCertificateHelper.GetX509Certificate2()
                        });
                    }
#endif
                })
#if NETSTANDARD1_3
                .UseUrls(_urls)
#endif
                .Build();

            return RunHost(_cts.Token);
        }

        private Task RunHost(CancellationToken token)
        {
            try
            {
                var appLifetime = (IApplicationLifetime)_host.Services.GetService(typeof(IApplicationLifetime));
                appLifetime.ApplicationStarted.Register(() =>
                {
                    IsStarted = true;
                });

#if NETSTANDARD1_3
                _logger.Info("WireMock.Net server using netstandard1.3");
#elif NETSTANDARD2_0
                _logger.Info("WireMock.Net server using netstandard2.0");
#elif NET46
                _logger.Info("WireMock.Net server using .net 4.6.1 or higher");
#endif
#if NETSTANDARD1_3
                return Task.Run(() =>
                {
                    _host.Run(token);
                });
#else
                return _host.RunAsync(token);
#endif
            }
            catch (Exception e)
            {
                _runningException = e;
                _logger.Error(e.ToString());

                IsStarted = false;

                return Task.CompletedTask;
            }
        }

        public Task StopAsync()
        {
            _cts.Cancel();

            IsStarted = false;
#if NETSTANDARD1_3
            return Task.FromResult(true);
#else
            return _host.StopAsync();
#endif
        }
    }
}
#endif
