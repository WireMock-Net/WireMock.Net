// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Exceptions;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Owin;
using WireMock.RequestBuilders;
using WireMock.ResponseProviders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Validation;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class WireMockServer : IDisposable
    {
        private const int ServerStartDelayInMs = 100;

        private readonly IWireMockServerSettings _settings;
        private readonly IOwinSelfHost _httpServer;
        private readonly IWireMockMiddlewareOptions _options = new WireMockMiddlewareOptions();
        private readonly MappingConverter _mappingConverter;
        private readonly MatcherMapper _matcherMapper;

        /// <summary>
        /// Gets a value indicating whether this server is started.
        /// </summary>
        [PublicAPI]
        public bool IsStarted => _httpServer != null && _httpServer.IsStarted;

        /// <summary>
        /// Gets the ports.
        /// </summary>
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
        public IEnumerable<IMapping> Mappings => _options.Mappings.Values.ToArray();

        /// <summary>
        /// Gets the mappings as MappingModels.
        /// </summary>
        [PublicAPI]
        public IEnumerable<MappingModel> MappingModels => ToMappingModels();

        /// <summary>
        /// Gets the scenarios.
        /// </summary>
        [PublicAPI]
        public ConcurrentDictionary<string, ScenarioState> Scenarios => new ConcurrentDictionary<string, ScenarioState>(_options.Scenarios);

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_httpServer != null)
            {
                _httpServer.StopAsync();
            }
        }
        #endregion

        #region Start/Stop
        /// <summary>
        /// Starts the specified settings.
        /// </summary>
        /// <param name="settings">The WireMockServerSettings.</param>
        /// <returns>The <see cref="WireMockServer"/>.</returns>
        [PublicAPI]
        public static WireMockServer Start([NotNull] IWireMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            return new WireMockServer(settings);
        }

        /// <summary>
        /// Start this WireMockServer.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ssl">The SSL support.</param>
        /// <returns>The <see cref="WireMockServer"/>.</returns>
        [PublicAPI]
        public static WireMockServer Start([CanBeNull] int? port = 0, bool ssl = false)
        {
            return new WireMockServer(new WireMockServerSettings
            {
                Port = port,
                UseSSL = ssl
            });
        }

        /// <summary>
        /// Start this WireMockServer.
        /// </summary>
        /// <param name="urls">The urls to listen on.</param>
        /// <returns>The <see cref="WireMockServer"/>.</returns>
        [PublicAPI]
        public static WireMockServer Start(params string[] urls)
        {
            Check.NotNullOrEmpty(urls, nameof(urls));

            return new WireMockServer(new WireMockServerSettings
            {
                Urls = urls
            });
        }

        /// <summary>
        /// Start this WireMockServer with the admin interface.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ssl">The SSL support.</param>
        /// <returns>The <see cref="WireMockServer"/>.</returns>
        [PublicAPI]
        public static WireMockServer StartWithAdminInterface(int? port = 0, bool ssl = false)
        {
            return new WireMockServer(new WireMockServerSettings
            {
                Port = port,
                UseSSL = ssl,
                StartAdminInterface = true
            });
        }

        /// <summary>
        /// Start this WireMockServer with the admin interface.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>The <see cref="WireMockServer"/>.</returns>
        [PublicAPI]
        public static WireMockServer StartWithAdminInterface(params string[] urls)
        {
            Check.NotNullOrEmpty(urls, nameof(urls));

            return new WireMockServer(new WireMockServerSettings
            {
                Urls = urls,
                StartAdminInterface = true
            });
        }

        /// <summary>
        /// Start this WireMockServer with the admin interface and read static mappings.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>The <see cref="WireMockServer"/>.</returns>
        [PublicAPI]
        public static WireMockServer StartWithAdminInterfaceAndReadStaticMappings(params string[] urls)
        {
            Check.NotNullOrEmpty(urls, nameof(urls));

            return new WireMockServer(new WireMockServerSettings
            {
                Urls = urls,
                StartAdminInterface = true,
                ReadStaticMappings = true
            });
        }
        protected WireMockServer(IWireMockServerSettings settings)
        {
            _settings = settings;

            // Set default values if not provided
            _settings.Logger = settings.Logger ?? new WireMockNullLogger();
            _settings.FileSystemHandler = settings.FileSystemHandler ?? new LocalFileSystemHandler();

            _settings.Logger.Info("WireMock.Net by Stef Heyenrath (https://github.com/WireMock-Net/WireMock.Net)");
            _settings.Logger.Debug("WireMock.Net server settings {0}", JsonConvert.SerializeObject(settings, Formatting.Indented));

            HostUrlOptions urlOptions;
            if (settings.Urls != null)
            {
                urlOptions = new HostUrlOptions
                {
                    Urls = settings.Urls
                };
            }
            else
            {
                urlOptions = new HostUrlOptions
                {
                    UseSSL = settings.UseSSL == true,
                    Port = settings.Port
                };
            }

            _options.FileSystemHandler = _settings.FileSystemHandler;
            _options.PreWireMockMiddlewareInit = _settings.PreWireMockMiddlewareInit;
            _options.PostWireMockMiddlewareInit = _settings.PostWireMockMiddlewareInit;
            _options.Logger = _settings.Logger;
            _options.DisableJsonBodyParsing = _settings.DisableJsonBodyParsing;

            _matcherMapper = new MatcherMapper(_settings);
            _mappingConverter = new MappingConverter(_matcherMapper);

#if USE_ASPNETCORE
            _httpServer = new AspNetCoreSelfHost(_options, urlOptions);
#else
            _httpServer = new OwinSelfHost(_options, urlOptions);
#endif
            var startTask = _httpServer.StartAsync();

            using (var ctsStartTimeout = new CancellationTokenSource(settings.StartTimeout))
            {
                while (!_httpServer.IsStarted)
                {
                    // Throw exception if service start fails
                    if (_httpServer.RunningException != null)
                    {
                        throw new WireMockException($"Service start failed with error: {_httpServer.RunningException.Message}", _httpServer.RunningException);
                    }

                    if (ctsStartTimeout.IsCancellationRequested)
                    {
                        // In case of an aggregate exception, throw the exception.
                        if (startTask.Exception != null)
                        {
                            throw new WireMockException($"Service start failed with error: {startTask.Exception.Message}", startTask.Exception);
                        }

                        // Else throw TimeoutException
                        throw new TimeoutException($"Service start timed out after {TimeSpan.FromMilliseconds(settings.StartTimeout)}");
                    }

                    ctsStartTimeout.Token.WaitHandle.WaitOne(ServerStartDelayInMs);
                }

                Urls = _httpServer.Urls.ToArray();
                Ports = _httpServer.Ports;
            }

            if (settings.AllowBodyForAllHttpMethods == true)
            {
                _options.AllowBodyForAllHttpMethods = _settings.AllowBodyForAllHttpMethods;
                _settings.Logger.Info("AllowBodyForAllHttpMethods is set to True");
            }

            if (settings.AllowOnlyDefinedHttpStatusCodeInResponse == true)
            {
                _options.AllowOnlyDefinedHttpStatusCodeInResponse = _settings.AllowOnlyDefinedHttpStatusCodeInResponse;
                _settings.Logger.Info("AllowOnlyDefinedHttpStatusCodeInResponse is set to True");
            }

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

            if (settings.WatchStaticMappings == true)
            {
                WatchStaticMappings();
            }

            if (settings.ProxyAndRecordSettings != null)
            {
                InitProxyAndRecord(settings);
            }

            if (settings.RequestLogExpirationDuration != null)
            {
                SetRequestLogExpirationDuration(settings.RequestLogExpirationDuration);
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
            var result = _httpServer?.StopAsync();
            result?.Wait(); // wait for stop to actually happen
        }
        #endregion

        /// <summary>
        /// Adds the catch all mapping.
        /// </summary>
        [PublicAPI]
        public void AddCatchAllMapping()
        {
            Given(Request.Create().WithPath("/*").UsingAnyMethod())
                .WithGuid(Guid.Parse("90008000-0000-4444-a17e-669cd84f1f05"))
                .AtPriority(1000)
                .RespondWith(new DynamicResponseProvider(request => ResponseMessageBuilder.Create("No matching mapping found", 404)));
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
            foreach (var nonAdmin in _options.Mappings.ToArray().Where(m => !m.Value.IsAdminInterface))
            {
                _options.Mappings.TryRemove(nonAdmin.Key, out _);
            }
        }

        /// <summary>
        /// Deletes the mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        [PublicAPI]
        public bool DeleteMapping(Guid guid)
        {
            // Check a mapping exists with the same GUID, if so, remove it.
            if (_options.Mappings.ContainsKey(guid))
            {
                return _options.Mappings.TryRemove(guid, out _);
            }

            return false;
        }

        private bool DeleteMapping(string path)
        {
            // Check a mapping exists with the same path, if so, remove it.
            var mapping = _options.Mappings.ToArray().FirstOrDefault(entry => string.Equals(entry.Value.Path, path, StringComparison.OrdinalIgnoreCase));
            return DeleteMapping(mapping.Key);
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
        public void AllowPartialMapping(bool allow = true)
        {
            _settings.Logger.Info("AllowPartialMapping is set to {0}", allow);
            _options.AllowPartialMapping = allow;
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
            _options.AuthorizationMatcher = new RegexMatcher(MatchBehaviour.AcceptOnMatch, "^(?i)BASIC " + authorization + "$");
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
        /// <param name="saveToFile">Optional boolean to indicate if this mapping should be saved as static mapping file.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        [PublicAPI]
        public IRespondWithAProvider Given(IRequestMatcher requestMatcher, bool saveToFile = false)
        {
            return new RespondWithAProvider(RegisterMapping, requestMatcher, _settings, saveToFile);
        }

        private void RegisterMapping(IMapping mapping, bool saveToFile)
        {
            // Check a mapping exists with the same Guid, if so, replace it.
            if (_options.Mappings.ContainsKey(mapping.Guid))
            {
                _options.Mappings[mapping.Guid] = mapping;
            }
            else
            {
                _options.Mappings.TryAdd(mapping.Guid, mapping);
            }

            if (saveToFile)
            {
                SaveMappingToFile(mapping);
            }
        }
    }
}