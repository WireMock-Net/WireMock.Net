using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Http;
using WireMock.Matchers.Request;
using WireMock.Validation;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class FluentMockServer
    {
        /// <summary>
        /// The _http server.
        /// </summary>
        private readonly TinyHttpServer _httpServer;

        /// <summary>
        /// The _mappings.
        /// </summary>
        private readonly IList<Mapping> _mappings = new List<Mapping>();

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
        /// Gets the routes.
        /// </summary>
        public IEnumerable<Mapping> Mappings
        {
            get
            {
                lock (((ICollection)_mappings).SyncRoot)
                {
                    return new ReadOnlyCollection<Mapping>(_mappings);
                }
            }
        }

        /// <summary>
        /// Start this FluentMockServer.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ssl">The SSL support.</param>
        /// <returns>The <see cref="FluentMockServer"/>.</returns>
        [PublicAPI]
        public static FluentMockServer Start(int port = 0, bool ssl = false)
        {
            Check.Condition(port, p => p >= 0, nameof(port));

            if (port == 0)
                port = Ports.FindFreeTcpPort();

            return new FluentMockServer(false, port, ssl);
        }

        /// <summary>
        /// Start this FluentMockServer with the admin interface.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ssl">The SSL support.</param>
        /// <returns>The <see cref="FluentMockServer"/>.</returns>
        [PublicAPI]
        public static FluentMockServer StartWithAdminInterface(int port = 0, bool ssl = false)
        {
            Check.Condition(port, p => p >= 0, nameof(port));

            if (port == 0)
                port = Ports.FindFreeTcpPort();

            return new FluentMockServer(true, port, ssl);
        }

        private FluentMockServer(bool startAdmin, int port, bool ssl)
        {
            string protocol = ssl ? "https" : "http";
            _httpServer = new TinyHttpServer(protocol + "://localhost:" + port + "/", HandleRequestAsync);
            Port = port;
            _httpServer.Start();

            if (startAdmin)
            {
                InitAdmin();
            }
        }

        /// <summary>
        /// Stop this server.
        /// </summary>
        public void Stop()
        {
            _httpServer.Stop();
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

            lock (((ICollection)_mappings).SyncRoot)
            {
                _mappings.Clear();
            }
        }

        /// <summary>
        /// The search logs for.
        /// </summary>
        /// <param name="spec">
        /// The matcher.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<RequestMessage> SearchLogsFor(IRequestMatcher spec)
        {
            lock (((ICollection)_requestLogs).SyncRoot)
            {
                return _requestLogs.Where(spec.IsMatch);
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
        /// The given.
        /// </summary>
        /// <param name="requestMatcher">The request matcher.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        public IRespondWithAProvider Given(IRequestMatcher requestMatcher)
        {
            return new RespondWithAProvider(RegisterMapping, requestMatcher);
        }

        /// <summary>
        /// The register mapping.
        /// </summary>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        private void RegisterMapping(Mapping mapping)
        {
            lock (((ICollection)_mappings).SyncRoot)
            {
                _mappings.Add(mapping);
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
        /// <param name="ctx">The HttpListenerContext.</param>
        private async void HandleRequestAsync(HttpListenerContext ctx)
        {
            lock (_syncRoot)
            {
                Task.Delay(_requestProcessingDelay).Wait();
            }

            var request = _requestMapper.Map(ctx.Request);
            LogRequest(request);

            try
            {
                var targetRoute = _mappings.FirstOrDefault(route => route.IsRequestHandled(request));
                if (targetRoute == null)
                {
                    ctx.Response.StatusCode = 404;

                    byte[] content = Encoding.UTF8.GetBytes("No mapping found");
                    ctx.Response.OutputStream.Write(content, 0, content.Length);
                }
                else
                {
                    var response = await targetRoute.ResponseTo(request);
                    _responseMapper.Map(response, ctx.Response);
                }
            }
            catch (Exception ex)
            {
                ctx.Response.StatusCode = 500;

                byte[] content = Encoding.UTF8.GetBytes(ex.ToString());
                ctx.Response.OutputStream.Write(content, 0, content.Length);
            }
            finally
            {
                ctx.Response.Close();
            }
        }
    }
}
