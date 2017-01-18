using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WireMock.Http;

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
namespace WireMock
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public class FluentMockServer
    {
        /// <summary>
        /// The _http server.
        /// </summary>
        private readonly TinyHttpServer _httpServer;

        /// <summary>
        /// The _routes.
        /// </summary>
        private readonly IList<Route> _routes = new List<Route>();

        /// <summary>
        /// The _request logs.
        /// </summary>
        private readonly IList<RequestMessage> _requestLogs = new List<RequestMessage>();

        /// <summary>
        /// The _request mapper.
        /// </summary>
        private readonly HttpListenerRequestMapper _requestMapper = new HttpListenerRequestMapper();

        /// <summary>
        /// The _response mapper.
        /// </summary>
        private readonly HttpListenerResponseMapper _responseMapper = new HttpListenerResponseMapper();

        /// <summary>
        /// The _sync root.
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// The _request processing delay.
        /// </summary>
        private TimeSpan _requestProcessingDelay = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentMockServer"/> class.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="ssl">
        /// The SSL support.
        /// </param>
        private FluentMockServer(int port, bool ssl)
        {
            string protocol = ssl ? "https" : "http";
            _httpServer = new TinyHttpServer(protocol + "://localhost:" + port + "/", HandleRequest);
            Port = port;
            _httpServer.Start();
        }

        /// <summary>
        /// The RespondWithAProvider interface.
        /// </summary>
        public interface IRespondWithAProvider
        {
            /// <summary>
            /// The respond with.
            /// </summary>
            /// <param name="provider">
            /// The provider.
            /// </param>
            void RespondWith(IProvideResponses provider);
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the request logs.
        /// </summary>
        public IEnumerable<RequestMessage> RequestLogs
        {
            get
            {
                lock (((ICollection)_requestLogs).SyncRoot)
                {
                    return new ReadOnlyCollection<RequestMessage>(_requestLogs);
                }
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="ssl">
        /// The SSL support.
        /// </param>
        /// <returns>
        /// The <see cref="FluentMockServer"/>.
        /// </returns>
        public static FluentMockServer Start(int port = 0, bool ssl = false)
        {
            if (port == 0)
            {
                port = Ports.FindFreeTcpPort();
            }

            return new FluentMockServer(port, ssl);
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public void Reset()
        {
            lock (((ICollection)_requestLogs).SyncRoot)
            {
                _requestLogs.Clear();
            }

            lock (((ICollection)_routes).SyncRoot)
            {
                _routes.Clear();
            }
        }

        /// <summary>
        /// The search logs for.
        /// </summary>
        /// <param name="spec">
        /// The spec.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<RequestMessage> SearchLogsFor(ISpecifyRequests spec)
        {
            lock (((ICollection)_requestLogs).SyncRoot)
            {
                return _requestLogs.Where(spec.IsSatisfiedBy);
            }
        }

        /// <summary>
        /// The add request processing delay.
        /// </summary>
        /// <param name="delay">
        /// The delay.
        /// </param>
        public void AddRequestProcessingDelay(TimeSpan delay)
        {
            lock (_syncRoot)
            {
                _requestProcessingDelay = delay;
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            _httpServer.Stop();
        }

        /// <summary>
        /// The given.
        /// </summary>
        /// <param name="requestSpec">
        /// The request spec.
        /// </param>
        /// <returns>
        /// The <see cref="IRespondWithAProvider"/>.
        /// </returns>
        public IRespondWithAProvider Given(ISpecifyRequests requestSpec)
        {
            return new RespondWithAProvider(RegisterRoute, requestSpec);
        }

        /// <summary>
        /// The register route.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        private void RegisterRoute(Route route)
        {
            lock (((ICollection)_routes).SyncRoot)
            {
                _routes.Add(route);
            }
        }

        /// <summary>
        /// The log request.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        private void LogRequest(RequestMessage requestMessage)
        {
            lock (((ICollection)_requestLogs).SyncRoot)
            {
                _requestLogs.Add(requestMessage);
            }
        }

        /// <summary>
        /// The handle request.
        /// </summary>
        /// <param name="ctx">
        /// The context.
        /// </param>
        private async void HandleRequest(HttpListenerContext ctx)
        {
            lock (_syncRoot)
            {
                Task.Delay(_requestProcessingDelay).Wait();
            }

            var request = _requestMapper.Map(ctx.Request);
            LogRequest(request);
            var targetRoute = _routes.FirstOrDefault(route => route.IsRequestHandled(request));
            if (targetRoute == null)
            {
                ctx.Response.StatusCode = 404;
                var content = Encoding.UTF8.GetBytes("<html><body>Mock Server: page not found</body></html>");
                ctx.Response.OutputStream.Write(content, 0, content.Length);
            }
            else
            {
                var response = await targetRoute.ResponseTo(request);

                _responseMapper.Map(response, ctx.Response);
            }

            ctx.Response.Close();
        }

        /// <summary>
        /// The respond with a provider.
        /// </summary>
        private class RespondWithAProvider : IRespondWithAProvider
        {
            /// <summary>
            /// The _registration callback.
            /// </summary>
            private readonly RegistrationCallback _registrationCallback;

            /// <summary>
            /// The _request spec.
            /// </summary>
            private readonly ISpecifyRequests _requestSpec;

            /// <summary>
            /// Initializes a new instance of the <see cref="RespondWithAProvider"/> class.
            /// </summary>
            /// <param name="registrationCallback">
            /// The registration callback.
            /// </param>
            /// <param name="requestSpec">
            /// The request spec.
            /// </param>
            public RespondWithAProvider(RegistrationCallback registrationCallback, ISpecifyRequests requestSpec)
            {
                _registrationCallback = registrationCallback;
                _requestSpec = requestSpec;
            }

            /// <summary>
            /// The respond with.
            /// </summary>
            /// <param name="provider">
            /// The provider.
            /// </param>
            public void RespondWith(IProvideResponses provider)
            {
                _registrationCallback(new Route(_requestSpec, provider));
            }
        }
    }
}
