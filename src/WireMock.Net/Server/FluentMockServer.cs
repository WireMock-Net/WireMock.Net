using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Http;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.Validation;
using System.Threading;
#if NET45
using System.Net;
#else
using System.Net.Http;
#endif

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class FluentMockServer : IDisposable
    {
        private readonly TinyHttpServer _httpServer;

        private IList<Mapping> _mappings = new List<Mapping>();

        private readonly IList<LogEntry> _logEntries = new List<LogEntry>();

        private readonly HttpListenerRequestMapper _requestMapper = new HttpListenerRequestMapper();

        private readonly HttpListenerResponseMapper _responseMapper = new HttpListenerResponseMapper();

        private readonly object _syncRoot = new object();

        private TimeSpan? _requestProcessingDelay;

        private bool _allowPartialMapping;

        private IMatcher _authorizationMatcher;

        /// <summary>
        /// Gets the ports.
        /// </summary>
        /// <value>
        /// The ports.
        /// </value>
        [PublicAPI]
        public List<int> Ports { get; }

        /// <summary>
        /// Gets the urls.
        /// </summary>
        [PublicAPI]
        public string[] Urls { get; }

        /// <summary>
        /// Gets the request logs.
        /// </summary>
        [PublicAPI]
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
        /// The search log-entries based on matchers.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        [PublicAPI]
        public IEnumerable<LogEntry> FindLogEntries([NotNull] params IRequestMatcher[] matchers)
        {
            lock (((ICollection)_logEntries).SyncRoot)
            {
                var results = new Dictionary<LogEntry, RequestMatchResult>();

                foreach (var log in _logEntries)
                {
                    var requestMatchResult = new RequestMatchResult();
                    foreach (var matcher in matchers)
                    {
                        matcher.GetMatchingScore(log.RequestMessage, requestMatchResult);
                    }

                    if (requestMatchResult.AverageTotalScore > 0.99)
                        results.Add(log, requestMatchResult);
                }

                return new ReadOnlyCollection<LogEntry>(results.OrderBy(x => x.Value).Select(x => x.Key).ToList());
            }
        }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        [PublicAPI]
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
        /// Starts the specified settings.
        /// </summary>
        /// <param name="settings">The FluentMockServerSettings.</param>
        /// <returns>The <see cref="FluentMockServer"/>.</returns>
        [PublicAPI]
        public static FluentMockServer Start(FluentMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            return new FluentMockServer(settings);
        }

        /// <summary>
        /// Start this FluentMockServer.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ssl">The SSL support.</param>
        /// <returns>The <see cref="FluentMockServer"/>.</returns>
        [PublicAPI]
        public static FluentMockServer Start([CanBeNull] int? port = 0, bool ssl = false)
        {
            return new FluentMockServer(new FluentMockServerSettings
            {
                Port = port,
                UseSSL = ssl
            });
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

            return new FluentMockServer(new FluentMockServerSettings
            {
                Urls = urls
            });
        }

        /// <summary>
        /// Start this FluentMockServer with the admin interface.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ssl">The SSL support.</param>
        /// <returns>The <see cref="FluentMockServer"/>.</returns>
        [PublicAPI]
        public static FluentMockServer StartWithAdminInterface(int? port = 0, bool ssl = false)
        {
            return new FluentMockServer(new FluentMockServerSettings
            {
                Port = port,
                UseSSL = ssl,
                StartAdminInterface = true
            });
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

            return new FluentMockServer(new FluentMockServerSettings
            {
                Urls = urls,
                StartAdminInterface = true
            });
        }

        /// <summary>
        /// Start this FluentMockServer with the admin interface and read static mappings.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>The <see cref="FluentMockServer"/>.</returns>
        [PublicAPI]
        public static FluentMockServer StartWithAdminInterfaceAndReadStaticMappings(params string[] urls)
        {
            Check.NotEmpty(urls, nameof(urls));

            return new FluentMockServer(new FluentMockServerSettings
            {
                Urls = urls,
                StartAdminInterface = true,
                ReadStaticMappings = true
            });
        }

        private FluentMockServer(FluentMockServerSettings settings)
        {
            if (settings.Urls != null)
            {
                Urls = settings.Urls;
            }
            else
            {
                int port = settings.Port > 0 ? settings.Port.Value : PortUtil.FindFreeTcpPort();
                Urls = new[] { (settings.UseSSL == true ? "https" : "http") + "://localhost:" + port + "/" };
            }

            _httpServer = new TinyHttpServer(HandleRequestAsync, Urls);
            Ports = _httpServer.Ports;

            _httpServer.Start();

            if (settings.StartAdminInterface == true)
            {
                InitAdmin();
            }

            if (settings.ReadStaticMappings == true)
            {
                ReadStaticMappings();
            }
        }

        /// <summary>
        /// Adds the catch all mapping.
        /// </summary>
        [PublicAPI]
        public void AddCatchAllMapping()
        {
            Given(Request.Create().WithPath("/*").UsingAnyVerb())
                .WithGuid(Guid.Parse("90008000-0000-4444-a17e-669cd84f1f05"))
                .AtPriority(1000)
                .RespondWith(new DynamicResponseProvider(request => new ResponseMessage { StatusCode = 404, Body = "No matching mapping found" }));
        }

        /// <summary>
        /// Stop this server.
        /// </summary>
        [PublicAPI]
        public void Stop()
        {
            _httpServer?.Stop();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_httpServer != null && _httpServer.IsStarted)
            {
                _httpServer.Stop();
            }
        }

        /// <summary>
        /// Resets LogEntries and Mappings.
        /// </summary>
        [PublicAPI]
        public void Reset()
        {
            ResetLogEntries();

            ResetMappings();
        }

        /// <summary>
        /// Resets the LogEntries.
        /// </summary>
        [PublicAPI]
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
        [PublicAPI]
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
        /// The add request processing delay.
        /// </summary>
        /// <param name="delay">
        /// The delay.
        /// </param>
        [PublicAPI]
        public void AddGlobalProcessingDelay(TimeSpan delay)
        {
            lock (_syncRoot)
            {
                _requestProcessingDelay = delay;
            }
        }

        /// <summary>
        /// Allows the partial mapping.
        /// </summary>
        [PublicAPI]
        public void AllowPartialMapping()
        {
            lock (_syncRoot)
            {
                _allowPartialMapping = true;
            }
        }

        /// <summary>
        /// Sets the basic authentication.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        [PublicAPI]
        public void SetBasicAuthentication([NotNull] string username, [NotNull] string password)
        {
            Check.NotNull(username, nameof(username));
            Check.NotNull(password, nameof(password));

            string authorization = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            _authorizationMatcher = new RegexMatcher("^(?i)BASIC " + authorization + "$");
        }

        /// <summary>
        /// The given.
        /// </summary>
        /// <param name="requestMatcher">The request matcher.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        [PublicAPI]
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
        /// <param name="cancel">The CancellationToken.</param>
        private async void HandleRequestAsync(HttpListenerContext ctx, CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return;

            if (_requestProcessingDelay > TimeSpan.Zero)
            {
                lock (_syncRoot)
                {
                    Task.Delay(_requestProcessingDelay.Value, cancel).Wait(cancel);
                }
            }

            var request = _requestMapper.Map(ctx.Request);

            ResponseMessage response = null;
            Mapping targetMapping = null;
            RequestMatchResult requestMatchResult = null;
            try
            {
                var mappings = _mappings
                    .Select(m => new { Mapping = m, MatchResult = m.IsRequestHandled(request) })
                    .ToList();

                if (_allowPartialMapping)
                {
                    var partialMappings = mappings
                        .Where(pm => pm.Mapping.IsAdminInterface && pm.MatchResult.IsPerfectMatch || !pm.Mapping.IsAdminInterface)
                        .OrderBy(m => m.MatchResult)
                        .ThenBy(m => m.Mapping.Priority)
                        .ToList();

                    var bestPartialMatch = partialMappings.FirstOrDefault(pm => pm.MatchResult.AverageTotalScore > 0.0);

                    targetMapping = bestPartialMatch?.Mapping;
                    requestMatchResult = bestPartialMatch?.MatchResult;
                }
                else
                {
                    var perfectMatch = mappings
                        .OrderBy(m => m.Mapping.Priority)
                        .FirstOrDefault(m => m.MatchResult.IsPerfectMatch);

                    targetMapping = perfectMatch?.Mapping;
                    requestMatchResult = perfectMatch?.MatchResult;
                }

                if (targetMapping == null)
                {
                    response = new ResponseMessage { StatusCode = 404, Body = "No matching mapping found" };
                    return;
                }

                if (targetMapping.IsAdminInterface && _authorizationMatcher != null)
                {
                    string authorization;
                    bool present = request.Headers.TryGetValue("Authorization", out authorization);
                    if (!present || _authorizationMatcher.IsMatch(authorization) < 1.0)
                    {
                        response = new ResponseMessage { StatusCode = 401 };
                        return;
                    }
                }

                response = await targetMapping.ResponseTo(request);
            }
            catch (Exception ex)
            {
                response = new ResponseMessage { StatusCode = 500, Body = ex.ToString() };
            }
            finally
            {
                var log = new LogEntry
                {
                    Guid = Guid.NewGuid(),
                    RequestMessage = request,
                    ResponseMessage = response,
                    MappingGuid = targetMapping?.Guid,
                    MappingTitle = targetMapping?.Title,
                    RequestMatchResult = requestMatchResult
                };

                LogRequest(log);

                _responseMapper.Map(response, ctx.Response);
                ctx.Response.Close();
            }
        }
    }
}