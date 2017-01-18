using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace WireMock.Http
{
    /// <summary>
    /// The tiny http server.
    /// </summary>
    public class TinyHttpServer
    {
        /// <summary>
        /// The _http handler.
        /// </summary>
        private readonly Action<HttpListenerContext> _httpHandler;

        /// <summary>
        /// The _listener.
        /// </summary>
        private readonly HttpListener _listener;

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        private CancellationTokenSource _cts;

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyHttpServer"/> class.
        /// </summary>
        /// <param name="urlPrefix">
        /// The url prefix.
        /// </param>
        /// <param name="httpHandler">
        /// The http handler.
        /// </param>
        public TinyHttpServer(string urlPrefix, Action<HttpListenerContext> httpHandler)
        {
            _httpHandler = httpHandler;

            // Create a listener.
            _listener = new HttpListener();
            _listener.Prefixes.Add(urlPrefix);
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            _listener.Start();
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
        /// The stop.
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
