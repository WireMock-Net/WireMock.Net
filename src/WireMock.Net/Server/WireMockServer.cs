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
    public partial class WireMockServer : IWireMockServer
    {
        private const int ServerStartDelayInMs = 100;

        private readonly IWireMockServerSettings _settings;
        private readonly IOwinSelfHost _httpServer;
        private readonly IWireMockMiddlewareOptions _options = new WireMockMiddlewareOptions();
        private readonly MappingConverter _mappingConverter;
        private readonly MatcherMapper _matcherMapper;

        /// <inheritdoc cref="IWireMockServer.IsStarted" />
        [PublicAPI]
        public bool IsStarted => _httpServer != null && _httpServer.IsStarted;

        /// <inheritdoc cref="IWireMockServer.Ports" />
        [PublicAPI]
        public List<int> Ports { get; }

        /// <inheritdoc cref="IWireMockServer.Urls" />
        [PublicAPI]
        public string[] Urls { get; }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        [PublicAPI]
        public IEnumerable<IMapping> Mappings => _options.Mappings.Values.ToArray();

        /// <inheritdoc cref="IWireMockServer.MappingModels" />
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
        /// Starts this WireMockServer with the specified settings.
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

        /// <summary>
        /// Initializes a new instance of the <see cref="WireMockServer"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="WireMockException">
        /// Service start failed with error: {_httpServer.RunningException.Message}
        /// or
        /// Service start failed with error: {startTask.Exception.Message}
        /// </exception>
        /// <exception cref="TimeoutException">Service start timed out after {TimeSpan.FromMilliseconds(settings.StartTimeout)}</exception>
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
            _options.HandleRequestsSynchronously = settings.HandleRequestsSynchronously;

            if (settings.CustomCertificateDefined)
            {
                _options.X509StoreName = settings.CertificateSettings.X509StoreName;
                _options.X509StoreLocation = settings.CertificateSettings.X509StoreLocation;
                _options.X509ThumbprintOrSubjectName = settings.CertificateSettings.X509StoreThumbprintOrSubjectName;
                _options.X509CertificateFilePath = settings.CertificateSettings.X509CertificateFilePath;
                _options.X509CertificatePassword = settings.CertificateSettings.X509CertificatePassword;
            }

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

        /// <inheritdoc cref="IWireMockServer.Stop" />
        [PublicAPI]
        public void Stop()
        {
            var result = _httpServer?.StopAsync();
            result?.Wait(); // wait for stop to actually happen
        }
        #endregion

        /// <inheritdoc cref="IWireMockServer.AddCatchAllMapping" />
        [PublicAPI]
        public void AddCatchAllMapping()
        {
            Given(Request.Create().WithPath("/*").UsingAnyMethod())
                .WithGuid(Guid.Parse("90008000-0000-4444-a17e-669cd84f1f05"))
                .AtPriority(1000)
                .RespondWith(new DynamicResponseProvider(request => ResponseMessageBuilder.Create("No matching mapping found", 404)));
        }

        /// <inheritdoc cref="IWireMockServer.Reset" />
        [PublicAPI]
        public void Reset()
        {
            ResetLogEntries();

            ResetMappings();
        }

        /// <inheritdoc cref="IWireMockServer.ResetMappings" />
        [PublicAPI]
        public void ResetMappings()
        {
            foreach (var nonAdmin in _options.Mappings.ToArray().Where(m => !m.Value.IsAdminInterface))
            {
                _options.Mappings.TryRemove(nonAdmin.Key, out _);
            }
        }

        /// <inheritdoc cref="IWireMockServer.DeleteMapping" />
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

        /// <inheritdoc cref="IWireMockServer.AddGlobalProcessingDelay" />
        [PublicAPI]
        public void AddGlobalProcessingDelay(TimeSpan delay)
        {
            _options.RequestProcessingDelay = delay;
        }

        /// <inheritdoc cref="IWireMockServer.AllowPartialMapping" />
        [PublicAPI]
        public void AllowPartialMapping(bool allow = true)
        {
            _settings.Logger.Info("AllowPartialMapping is set to {0}", allow);
            _options.AllowPartialMapping = allow;
        }

        /// <inheritdoc cref="IWireMockServer.SetBasicAuthentication" />
        [PublicAPI]
        public void SetBasicAuthentication([NotNull] string username, [NotNull] string password)
        {
            Check.NotNull(username, nameof(username));
            Check.NotNull(password, nameof(password));

            string authorization = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            _options.AuthorizationMatcher = new RegexMatcher(MatchBehaviour.AcceptOnMatch, "^(?i)BASIC " + authorization + "$");
        }

        /// <inheritdoc cref="IWireMockServer.RemoveBasicAuthentication" />
        [PublicAPI]
        public void RemoveBasicAuthentication()
        {
            _options.AuthorizationMatcher = null;
        }

        /// <inheritdoc cref="IWireMockServer.SetMaxRequestLogCount" />
        [PublicAPI]
        public void SetMaxRequestLogCount([CanBeNull] int? maxRequestLogCount)
        {
            _options.MaxRequestLogCount = maxRequestLogCount;
        }

        /// <inheritdoc cref="IWireMockServer.SetRequestLogExpirationDuration" />
        [PublicAPI]
        public void SetRequestLogExpirationDuration([CanBeNull] int? requestLogExpirationDuration)
        {
            _options.RequestLogExpirationDuration = requestLogExpirationDuration;
        }

        /// <inheritdoc cref="IWireMockServer.ResetScenarios" />
        [PublicAPI]
        public void ResetScenarios()
        {
            _options.Scenarios.Clear();
        }

        /// <inheritdoc cref="IWireMockServer.WithMapping(MappingModel[])" />
        [PublicAPI]
        public IWireMockServer WithMapping(params MappingModel[] mappings)
        {
            foreach (var mapping in mappings)
            {
                ConvertMappingAndRegisterAsRespondProvider(mapping, mapping.Guid ?? Guid.NewGuid());
            }

            return this;
        }

        /// <inheritdoc cref="IWireMockServer.WithMapping(string)" />
        [PublicAPI]
        public IWireMockServer WithMapping(string mappings)
        {
            var mappingModels = DeserializeJsonToArray<MappingModel>(mappings);
            foreach (var mappingModel in mappingModels)
            {
                ConvertMappingAndRegisterAsRespondProvider(mappingModel, mappingModel.Guid ?? Guid.NewGuid());
            }

            return this;
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