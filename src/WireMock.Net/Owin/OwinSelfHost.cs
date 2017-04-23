using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Owin.Hosting;
using WireMock.Validation;
using Owin;

namespace WireMock.Owin
{
    internal class OwinSelfHost
    {
        private readonly WireMockMiddlewareOptions _options;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Thread _internalThread;

        public OwinSelfHost([NotNull] WireMockMiddlewareOptions options, [NotNull] params string[] uriPrefixes)
        {
            _options = options;
            Check.NotEmpty(uriPrefixes, nameof(uriPrefixes));

            foreach (string uriPrefix in uriPrefixes)
            {
                var uri = new Uri(uriPrefix);
                Urls.Add(uri);
                Ports.Add(uri.Port);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this server is started.
        /// </summary>
        /// <value>
        /// <c>true</c> if this server is started; otherwise, <c>false</c>.
        /// </value>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Gets the url.
        /// </summary>
        /// <value>
        /// The urls.
        /// </value>
        [PublicAPI]
        public List<Uri> Urls { get; } = new List<Uri>();

        /// <summary>
        /// Gets the ports.
        /// </summary>
        /// <value>
        /// The ports.
        /// </value>
        [PublicAPI]
        public List<int> Ports { get; } = new List<int>();

        [PublicAPI]
        public void Start()
        {
            //Task.Run(() =>
            //{
            //    Action<IAppBuilder> startup = app =>
            //    {
            //        app.UseWireMockMiddleware(_options);
            //    };

            //    var servers = new List<IDisposable>();
            //    foreach (var url in Urls)
            //    {
            //        servers.Add(WebApp.Start(url.ToString(), startup));
            //    }

            //    IsStarted = true;

            //    while (!_cts.IsCancellationRequested)
            //        Thread.Sleep(1000);

            //    IsStarted = false;

            //    foreach (var server in servers)
            //        server.Dispose();
            //},
            //_cts.Token);

            if (_internalThread != null)
                throw new InvalidOperationException("Cannot start a multiple threads.");

            _internalThread = new Thread(ThreadWorkInternal);
            _internalThread.Start();
        }

        [PublicAPI]
        public Task Stop()
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

        private void ThreadWorkInternal()
        {
            try
            {
                Action<IAppBuilder> startup = app =>
                {
                    app.UseWireMockMiddleware(_options);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _internalThread = null;
            }
        }
    }
}