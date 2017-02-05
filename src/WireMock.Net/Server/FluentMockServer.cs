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

        private IList<Mapping> _mappings = new List<Mapping>();

        private readonly IList<LogEntry> _logEntries = new List<LogEntry>();

        private readonly HttpListenerRequestMapper _requestMapper = new HttpListenerRequestMapper();

        private readonly HttpListenerResponseMapper _responseMapper = new HttpListenerResponseMapper();

        private readonly object _syncRoot = new object();

        private TimeSpan _requestProcessingDelay = TimeSpan.Zero;

        private bool _allowPartialMapping;

        /// <summary>
        /// Gets the ports.
        /// </summary>
        /// <value>
        /// The ports.
        /// </value>
        public List<int> Ports { get; }

        /// <summary>
        /// Gets the urls.
        /// </summary>
        public string[] Urls { get; }

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
                port = PortUtil.FindFreeTcpPort();

            return new FluentMockServer(false, port, ssl);
        }

        /// <summary>
        /// Start this FluentMockServer.
        /// </summary>
        /// <param name="urls">The urls to listen on.</param>
        /// <returns>The <see cref="FluentMockServer"/>.</returns>
        [PublicAPI]
        public static FluentMockServer Start(params string[] urls)
        {
            Check.NotEmpty(urls, nameof(urls));

            return new FluentMockServer(false, urls);
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
                port = PortUtil.FindFreeTcpPort();

            return new FluentMockServer(true, port, ssl);
        }

        /// <summary>
        /// Start this FluentMockServer with the admin interface.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>The <see cref="FluentMockServer"/>.</returns>
        [PublicAPI]
        public static FluentMockServer StartWithAdminInterface(params string[] urls)
        {
            Check.NotEmpty(urls, nameof(urls));

            return new FluentMockServer(true, urls);
        }

        private FluentMockServer(bool startAdminInterface, int port, bool ssl) : this(startAdminInterface, (ssl ? "https" : "http") + "://localhost:" + port + "/")
        {
        }

        private FluentMockServer(bool startAdminInterface, params string[] urls)
        {
            Urls = urls;

            _httpServer = new TinyHttpServer(HandleRequestAsync, urls);
            Ports = _httpServer.Ports;

            _httpServer.Start();

            if (startAdminInterface)
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
        /// Resets LogEntries and Mappings.
        /// </summary>
        public void Reset()
        {
            ResetLogEntries();

            ResetMappings();
        }

        /// <summary>
        /// Resets the LogEntries.
        /// </summary>
        public void ResetLogEntries()
        {
            lock (((ICollection)_logEntries).SyncRoot)
            {
                _logEntries.Clear();
            }
        }

        /// <summary>
        /// Deletes the mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        [PublicAPI]
        public bool DeleteLogEntry(Guid guid)
        {
            lock (((ICollection)_logEntries).SyncRoot)
            {
                // Check a logentry exists with the same GUID, if so, remove it.
                var existing = _logEntries.FirstOrDefault(m => m.Guid == guid);
                if (existing != null)
                {
                    _logEntries.Remove(existing);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Resets the Mappings.
        /// </summary>
        public void ResetMappings()
        {
            lock (((ICollection)_mappings).SyncRoot)
            {
                _mappings = _mappings.Where(m => m.Provider is DynamicResponseProvider).ToList();
            }
        }

        /// <summary>
        /// Deletes the mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        [PublicAPI]
        public bool DeleteMapping(Guid guid)
        {
            lock (((ICollection)_mappings).SyncRoot)
            {
                // Check a mapping exists with the same GUID, if so, remove it.
                var existingMapping = _mappings.FirstOrDefault(m => m.Guid == guid);
                if (existingMapping != null)
                {
                    _mappings.Remove(existingMapping);
                    return true;
                }

                return false;
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
                var requestMatchResult = new RequestMatchResult();
                return _logEntries.Where(log => matcher.IsMatch(log.RequestMessage, requestMatchResult) > 0.99);
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
        /// Allows the partial mapping.
        /// </summary>
        public void AllowPartialMapping()
        {
            lock (_syncRoot)
            {
                _allowPartialMapping = true;
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
                // Check a mapping exists with the same GUID, if so, remove it first.
                DeleteMapping(mapping.Guid);

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
            if (_requestProcessingDelay > TimeSpan.Zero)
            {
                lock (_syncRoot)
                {
                    Task.Delay(_requestProcessingDelay).Wait();
                }
            }

            var request = _requestMapper.Map(ctx.Request);

            ResponseMessage response = null;
            Mapping targetMapping = null;
            RequestMatchResult requestMatchResult = null;
            try
            {
                var possibleMatchingMappings = _mappings
                    .Select(m => new { Mapping = m, MatchResult = m.IsRequestHandled(request) })
                    .ToList();

                if (_allowPartialMapping)
                {
                    var orderedMappings = possibleMatchingMappings
                        .Where(pm =>
                            (pm.Mapping.Provider is DynamicResponseProvider && pm.MatchResult.IsPerfectMatch) ||
                            !(pm.Mapping.Provider is DynamicResponseProvider)
                        )
                        .OrderBy(m => m.MatchResult)
                        .ThenBy(m => m.Mapping.Priority)
                        .ToList();

                    var bestPartialMatch = orderedMappings.FirstOrDefault();

                    targetMapping = bestPartialMatch?.Mapping;
                    requestMatchResult = bestPartialMatch?.MatchResult;
                }
                else
                {
                    var perfectMatch = possibleMatchingMappings
                        .OrderBy(m => m.Mapping.Priority)
                        .FirstOrDefault(m => m.MatchResult.IsPerfectMatch);

                    targetMapping = perfectMatch?.Mapping;
                    requestMatchResult = perfectMatch?.MatchResult;
                }

                if (targetMapping == null)
                {
                    response = new ResponseMessage
                    {
                        StatusCode = 404,
                        Body = "No mapping found"
                    };
                }
                else
                {
                    response = await targetMapping.ResponseTo(request);
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
                    Guid = Guid.NewGuid(),
                    RequestMessage = request,
                    ResponseMessage = response,
                    MappingGuid = targetMapping?.Guid,
                    RequestMatchResult = requestMatchResult
                };

                LogRequest(log);

                _responseMapper.Map(response, ctx.Response);
                ctx.Response.Close();
            }
        }
    }
}
