using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Http;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Validation;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class FluentMockServer
    {
        private readonly TinyHttpServer _httpServer;

        private readonly IList<Mapping> _mappings = new List<Mapping>();

        private readonly IList<LogEntry> _logEntries = new List<LogEntry>();

        private readonly HttpListenerRequestMapper _requestMapper = new HttpListenerRequestMapper();

        private readonly HttpListenerResponseMapper _responseMapper = new HttpListenerResponseMapper();

        private readonly object _syncRoot = new object();

        private TimeSpan _requestProcessingDelay = TimeSpan.Zero;

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the request logs.
        /// </summary>
        public IEnumerable<LogEntry> LogEntries
        {
            get
            {
                lock (((ICollection)_logEntries).SyncRoot)
                {
                    return new ReadOnlyCollection<LogEntry>(_logEntries);
                }
            }
        }

        /// <summary>
        /// Gets the mappings.
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
            lock (((ICollection)_logEntries).SyncRoot)
            {
                _logEntries.Clear();
            }

            lock (((ICollection)_mappings).SyncRoot)
            {
                _mappings.Clear();
            }
        }

        /// <summary>
        /// The search logs for.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        public IEnumerable<LogEntry> SearchLogsFor(IRequestMatcher matcher)
        {
            lock (((ICollection)_logEntries).SyncRoot)
            {
                return _logEntries.Where(log => matcher.IsMatch(log.RequestMessage));
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
        /// <param name="entry">The request.</param>
        private void LogRequest(LogEntry entry)
        {
            lock (((ICollection)_logEntries).SyncRoot)
            {
                _logEntries.Add(entry);
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

            ResponseMessage response = null;

            try
            {
                var targetRoute = _mappings.FirstOrDefault(route => route.IsRequestHandled(request));
                if (targetRoute == null)
                {
                    response = new ResponseMessage
                    {
                        StatusCode = 404,
                        Body = "No mapping found"
                    };
                }
                else
                {
                    response = await targetRoute.ResponseTo(request);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseMessage
                {
                    StatusCode = 500,
                    Body = ex.ToString()
                };
            }
            finally
            {
                var log = new LogEntry
                {
                    RequestMessage = request,
                    ResponseMessage = response
                };

                LogRequest(log);

                _responseMapper.Map(response, ctx.Response);
                ctx.Response.Close();
            }
        }
    }
}
