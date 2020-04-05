﻿#if !USE_ASPNETCORE
using JetBrains.Annotations;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WireMock.Logging;
using WireMock.Owin.Mappers;
using WireMock.Validation;

namespace WireMock.Owin
{
    internal class OwinSelfHost : IOwinSelfHost
    {
        private readonly IWireMockMiddlewareOptions _options;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly IWireMockLogger _logger;

        private Exception _runningException;

        public OwinSelfHost([NotNull] IWireMockMiddlewareOptions options, [NotNull] HostUrlOptions urlOptions)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(urlOptions, nameof(urlOptions));

            _options = options;
            _logger = options.Logger ?? new WireMockConsoleLogger();

            foreach (var detail in urlOptions.GetDetails())
            {
                Urls.Add(detail.Url);
                Ports.Add(detail.Port);
            }
        }

        public bool IsStarted { get; private set; }

        public List<string> Urls { get; } = new List<string>();

        public List<int> Ports { get; } = new List<int>();

        public Exception RunningException => _runningException;

        [PublicAPI]
        public Task StartAsync()
        {
            return Task.Run(() =>
            {
                StartServers();
            }, _cts.Token);
        }

        [PublicAPI]
        public Task StopAsync()
        {
            _cts.Cancel();

            return Task.FromResult(true);
        }

        private void StartServers()
        {
#if NET46
            _logger.Info("WireMock.Net server using .net 4.6.1 or higher");
#else
            _logger.Info("WireMock.Net server using .net 4.5.x");
#endif
            var servers = new List<IDisposable>();

            try
            {
                var requestMapper = new OwinRequestMapper();
                var responseMapper = new OwinResponseMapper(_options);
                var matcher = new MappingMatcher(_options);

                Action<IAppBuilder> startup = app =>
                {
                    app.Use<GlobalExceptionMiddleware>(_options, responseMapper);
                    _options.PreWireMockMiddlewareInit?.Invoke(app);
                    app.Use<WireMockMiddleware>(_options, requestMapper, responseMapper, matcher);
                    _options.PostWireMockMiddlewareInit?.Invoke(app);
                };

                foreach (var url in Urls)
                {
                    servers.Add(WebApp.Start(url, startup));
                }

                IsStarted = true;

                // WaitHandle is signaled when the token is cancelled,
                // which will be more efficient than Thread.Sleep in while loop
                _cts.Token.WaitHandle.WaitOne();
            }
            catch (Exception e)
            {
                // Expose exception of starting host, otherwise it's hard to be troubleshooting if keeping quiet
                // For example, WebApp.Start will fail with System.MissingMemberException if Microsoft.Owin.Host.HttpListener.dll is being located
                // https://stackoverflow.com/questions/25090211/owin-httplistener-not-located/31369857
                _runningException = e;
                _logger.Error(e.ToString());
            }
            finally
            {
                IsStarted = false;
                // Dispose all servers in finally block to make sure clean up allocated resource on error happening
                servers.ForEach(s => s.Dispose());
            }
        }
    }
}
#endif