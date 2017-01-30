using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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
        /// Initializes a new instance of the <see cref="TinyHttpServer"/> class.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <param name="httpHandler">The http handler.</param>
        public TinyHttpServer(string[] urls, Action<HttpListenerContext> httpHandler)
        {
            _httpHandler = httpHandler;

            // Create a listener.
            _listener = new HttpListener();
            foreach (string urlPrefix in urls)
            {
                _listener.Prefixes.Add(urlPrefix);
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