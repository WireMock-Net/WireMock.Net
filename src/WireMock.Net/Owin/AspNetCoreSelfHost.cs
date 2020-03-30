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
        private readonly IWireMockLogger _logger;
        private readonly HostUrlOptions _urlOptions;

        private Exception _runningException;
        private IWebHost _host;

        public bool IsStarted { get; private set; }

        public List<string> Urls { get; } = new List<string>();

        public List<int> Ports { get; } = new List<int>();

        public Exception RunningException => _runningException;

        public AspNetCoreSelfHost([NotNull] IWireMockMiddlewareOptions options, [NotNull] HostUrlOptions urlOptions)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(urlOptions, nameof(urlOptions));

            _logger = options.Logger ?? new WireMockConsoleLogger();

            _options = options;
            _urlOptions = urlOptions;
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
                })
                .Configure(appBuilder =>
                {
                    appBuilder.UseMiddleware<GlobalExceptionMiddleware>();

                    _options.PreWireMockMiddlewareInit?.Invoke(appBuilder);

                    appBuilder.UseMiddleware<WireMockMiddleware>();

                    _options.PostWireMockMiddlewareInit?.Invoke(appBuilder);
                })
                .UseKestrel(options =>
                {
                    var urlDetails = _urlOptions.GetDetails();

#if NETSTANDARD1_3

                    var urls = urlDetails.Select(u => u.Url);
                    if (urls.Any(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                    {
                        options.UseHttps(PublicCertificateHelper.GetX509Certificate2());
                    }
#else
                    foreach (var detail in urlDetails)
                    {
                        if (detail.Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            options.Listen(System.Net.IPAddress.Any, detail.Port, listenOptions =>
                            {
                                listenOptions.UseHttps(); // PublicCertificateHelper.GetX509Certificate2()
                            });
                        }
                        else
                        {
                            options.Listen(System.Net.IPAddress.Any, detail.Port);
                        }
                    }
#endif
                })

#if NETSTANDARD1_3
                .UseUrls(_urlOptions.GetDetails().Select(u => u.Url).ToArray())
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
                    var addresses = _host.ServerFeatures
                        .Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()
                        .Addresses;

                    foreach (string address in addresses)
                    {
                        Urls.Add(address.Replace("0.0.0.0", "localhost"));

                        PortUtils.TryExtract(address, out string protocol, out string host, out int port);
                        Ports.Add(port);
                    }

                    IsStarted = true;
                });

#if NETSTANDARD1_3
                _logger.Info("WireMock.Net server using netstandard1.3");
#elif NETSTANDARD2_0
                _logger.Info("WireMock.Net server using netstandard2.0");
#elif NETSTANDARD2_1
                _logger.Info("WireMock.Net server using netstandard2.1");
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