#if !NETSTANDARD
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Validation;
using Owin;
using Microsoft.Owin.Hosting;

namespace WireMock.Owin
{
    internal class OwinSelfHost : IOwinSelfHost
    {
        private readonly WireMockMiddlewareOptions _options;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private System.Threading.Thread _internalThread;

        public OwinSelfHost([NotNull] WireMockMiddlewareOptions options, [NotNull] params string[] uriPrefixes)
        {
            Check.NotNull(options, nameof(options));
            Check.NotEmpty(uriPrefixes, nameof(uriPrefixes));

            foreach (string uriPrefix in uriPrefixes)
            {
                var uri = new Uri(uriPrefix);
                Urls.Add(uri);
                Ports.Add(uri.Port);
            }

            _options = options;
        }

        public bool IsStarted { get; private set; }

        public List<Uri> Urls { get; } = new List<Uri>();

        public List<int> Ports { get; } = new List<int>();

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

            var tcs = new TaskCompletionSource<bool>();
            var timer = new System.Timers.Timer(999);
            timer.Elapsed += (sender, e) =>
            {
                if (_internalThread == null)
                {
                    timer.Stop();
                    tcs.SetResult(true);
                }
            };
            timer.Start();

            return tcs.Task;
        }

        private void StartServers()
        {
            Action<IAppBuilder> startup = app =>
            {
                app.Use<WireMockMiddleware>(_options);
            };

            var servers = new List<IDisposable>();
            foreach (var url in Urls)
            {
                servers.Add(WebApp.Start(url.ToString(), startup));
            }

            IsStarted = true;

            while (!_cts.IsCancellationRequested)
                Thread.Sleep(1000);

            IsStarted = false;

            foreach (var server in servers)
                server.Dispose();
        }
    }
}
#endif