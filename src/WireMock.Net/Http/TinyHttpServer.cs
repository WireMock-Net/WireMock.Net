//using System;
//using System.Collections.Generic;
//#if NET45
//using System.Net;
//#else
//using System.Net.Http;
//#endif
//using System.Threading;
//using System.Threading.Tasks;
//using JetBrains.Annotations;
//using WireMock.Validation;

//namespace WireMock.Http
//{
//    /// <summary>
//    /// The tiny http server.
//    /// </summary>
//    public class TinyHttpServerOld
//    {
//        private readonly Action<HttpListenerContext, CancellationToken> _httpHandler;

//        private readonly HttpListener _listener;

//        private readonly CancellationTokenSource _cts;

//        /// <summary>
//        /// Gets a value indicating whether this server is started.
//        /// </summary>
//        /// <value>
//        /// <c>true</c> if this server is started; otherwise, <c>false</c>.
//        /// </value>
//        public bool IsStarted { get; private set; }

//        /// <summary>
//        /// Gets the url.
//        /// </summary>
//        /// <value>
//        /// The urls.
//        /// </value>
//        [PublicAPI]
//        public List<Uri> Urls { get; } = new List<Uri>();

//        /// <summary>
//        /// Gets the ports.
//        /// </summary>
//        /// <value>
//        /// The ports.
//        /// </value>
//        [PublicAPI]
//        public List<int> Ports { get; } = new List<int>();

//        /// <summary>
//        /// Initializes a new instance of the <see cref="TinyHttpServer"/> class.
//        /// </summary>
//        /// <param name="uriPrefixes">The uriPrefixes.</param>
//        /// <param name="httpHandler">The http handler.</param>
//        public TinyHttpServerOld([NotNull] Action<HttpListenerContext, CancellationToken> httpHandler, [NotNull] params string[] uriPrefixes)
//        {
//            Check.NotNull(httpHandler, nameof(httpHandler));
//            Check.NotEmpty(uriPrefixes, nameof(uriPrefixes));

//            _cts = new CancellationTokenSource();

//            _httpHandler = httpHandler;

//            // Create a listener.
//            _listener = new HttpListener();
//            foreach (string uriPrefix in uriPrefixes)
//            {
//                var uri = new Uri(uriPrefix);
//                Urls.Add(uri);
//                Ports.Add(uri.Port);

//                _listener.Prefixes.Add(uriPrefix);
//            }
//        }

//        /// <summary>
//        /// Start the server.
//        /// </summary>
//        [PublicAPI]
//        public void Start()
//        {
//            _listener.Start();

//            IsStarted = true;

//            Task.Run(
//                async () =>
//                    {
//                        //using (_listener)
//                        {
//                            while (!_cts.Token.IsCancellationRequested)
//                            {
//                                HttpListenerContext context = await _listener.GetContextAsync();
//                                _httpHandler(context, _cts.Token);
//                            }

//                            _listener.Stop();
//                            IsStarted = false;
//                        }
//                    },
//                _cts.Token);
//        }

//        /// <summary>
//        /// Stop the server.
//        /// </summary>
//        [PublicAPI]
//        public void Stop()
//        {
//            _listener?.Stop();

//            _cts.Cancel();
//        }
//    }
//}