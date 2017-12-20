using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.Settings;
using WireMock.Validation;
using WireMock.Owin;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class FluentMockServer : IDisposable
    {
        private const int ServerStartDelay = 100;
        private readonly IOwinSelfHost _httpServer;
        private readonly WireMockMiddlewareOptions _options = new WireMockMiddlewareOptions();

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
        /// Gets the mappings.
        /// </summary>
        [PublicAPI]
        public IEnumerable<Mapping> Mappings => new ReadOnlyCollection<Mapping>(_options.Mappings);

        /// <summary>
        /// Gets the scenarios.
        /// </summary>
        [PublicAPI]
        public IDictionary<string, object> Scenarios => new ConcurrentDictionary<string, object>(_options.Scenarios);

        #region Start/Stop
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

            _options.PreWireMockMiddlewareInit = settings.PreWireMockMiddlewareInit;
            _options.PostWireMockMiddlewareInit = settings.PostWireMockMiddlewareInit;

#if NETSTANDARD
            _httpServer = new AspNetCoreSelfHost(_options, Urls);
#else
            _httpServer = new OwinSelfHost(_options, Urls);
#endif
            Ports = _httpServer.Ports;

            _httpServer.StartAsync();

            // Fix for 'Bug: Server not listening after Start() returns (on macOS)'
            Task.Delay(ServerStartDelay).Wait();

            if (settings.AllowPartialMapping == true)
            {
                AllowPartialMapping();
            }

            if (settings.StartAdminInterface == true)
            {
                if (!string.IsNullOrEmpty(settings.AdminUsername) && !string.IsNullOrEmpty(settings.AdminPassword))
                {
                    SetBasicAuthentication(settings.AdminUsername, settings.AdminPassword);
                }

                InitAdmin();
            }

            if (settings.ReadStaticMappings == true)
            {
                ReadStaticMappings();
            }

            if (settings.ProxyAndRecordSettings != null)
            {
                InitProxyAndRecord(settings.ProxyAndRecordSettings);
            }

            if (settings.MaxRequestLogCount != null)
            {
                SetMaxRequestLogCount(settings.MaxRequestLogCount);
            }
        }

        /// <summary>
        /// Stop this server.
        /// </summary>
        [PublicAPI]
        public void Stop()
        {
            _httpServer?.StopAsync();
        }
        #endregion

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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_httpServer != null && _httpServer.IsStarted)
            {
                _httpServer.StopAsync();
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
        /// Resets the Mappings.
        /// </summary>
        [PublicAPI]
        public void ResetMappings()
        {
            _options.Mappings = _options.Mappings.Where(m => m.IsAdminInterface).ToList();
        }

        /// <summary>
        /// Deletes the mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        [PublicAPI]
        public bool DeleteMapping(Guid guid)
        {
            // Check a mapping exists with the same GUID, if so, remove it.
            var existingMapping = _options.Mappings.FirstOrDefault(m => m.Guid == guid);
            if (existingMapping != null)
            {
                _options.Mappings.Remove(existingMapping);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The add request processing delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        [PublicAPI]
        public void AddGlobalProcessingDelay(TimeSpan delay)
        {
            _options.RequestProcessingDelay = delay;
        }

        /// <summary>
        /// Allows the partial mapping.
        /// </summary>
        [PublicAPI]
        public void AllowPartialMapping()
        {
            _options.AllowPartialMapping = true;
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
            _options.AuthorizationMatcher = new RegexMatcher("^(?i)BASIC " + authorization + "$");
        }

        /// <summary>
        /// Removes the basic authentication.
        /// </summary>
        [PublicAPI]
        public void RemoveBasicAuthentication()
        {
            _options.AuthorizationMatcher = null;
        }

        /// <summary>
        /// Sets the maximum RequestLog count.
        /// </summary>
        /// <param name="maxRequestLogCount">The maximum RequestLog count.</param>
        [PublicAPI]
        public void SetMaxRequestLogCount([CanBeNull] int? maxRequestLogCount)
        {
            _options.MaxRequestLogCount = maxRequestLogCount;

        }

        /// <summary>
        /// Sets RequestLog expiration in hours.
        /// </summary>
        /// <param name="requestLogExpirationDuration">The RequestLog expiration in hours.</param>
        [PublicAPI]
        public void SetRequestLogExpirationDuration([CanBeNull] int? requestLogExpirationDuration)
        {
            _options.RequestLogExpirationDuration = requestLogExpirationDuration;
        }

        /// <summary>
        /// Resets the Scenarios.
        /// </summary>
        [PublicAPI]
        public void ResetScenarios()
        {
            _options.Scenarios.Clear();
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

        private void RegisterMapping(Mapping mapping)
        {
            // Check a mapping exists with the same Guid, if so, replace it.
            var existingMapping = _options.Mappings.FirstOrDefault(m => m.Guid == mapping.Guid);
            if (existingMapping != null)
            {
                _options.Mappings[_options.Mappings.IndexOf(existingMapping)] = mapping;
            }
            else
            {
                _options.Mappings.Add(mapping);
            }
        }
    }
}