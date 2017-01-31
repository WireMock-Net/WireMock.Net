using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Http
{
    /// <summary>
    /// The tiny http server.
    /// </summary>
    public class TinyHttpServer
    {
        private readonly Action<HttpListenerContext> _httpHandler;

        private readonly HttpListener _listener;

        private CancellationTokenSource _cts;

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
        public List<Uri> Urls { get; } = new List<Uri>();

        /// <summary>
        /// Gets the ports.
        /// </summary>
        /// <value>
        /// The ports.
        /// </value>
        public List<int> Ports { get; } = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyHttpServer"/> class.
        /// </summary>
        /// <param name="uriPrefixes">The uriPrefixes.</param>
        /// <param name="httpHandler">The http handler.</param>
        public TinyHttpServer([NotNull] Action<HttpListenerContext> httpHandler, [NotNull] params string[] uriPrefixes)
        {
            Check.NotNull(httpHandler, nameof(httpHandler));
            Check.NotEmpty(uriPrefixes, nameof(uriPrefixes));

            _httpHandler = httpHandler;

            // Create a listener.
            _listener = new HttpListener();
            foreach (string uriPrefix in uriPrefixes)
            {
                var uri = new Uri(uriPrefix);
                Urls.Add(uri);
                Ports.Add(uri.Port);

                _listener.Prefixes.Add(uriPrefix);
            }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            _listener.Start();
            IsStarted = true;

            _cts = new CancellationTokenSource();
            Task.Run(
                async () =>
                    {
                        using (_listener)
                        {
                            while (!_cts.Token.IsCancellationRequested)
                            {
                                HttpListenerContext context = await _listener.GetContextAsync();
                                _httpHandler(context);
                            }
                        }
                    },
                _cts.Token);
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
        }
    }
}