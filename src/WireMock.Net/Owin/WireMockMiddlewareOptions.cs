using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Util;
#if !USE_ASPNETCORE
using Owin;
#else
using IAppBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;
#endif

namespace WireMock.Owin
{
    internal class WireMockMiddlewareOptions : IWireMockMiddlewareOptions
    {
        public IWireMockLogger Logger { get; set; }

        public TimeSpan? RequestProcessingDelay { get; set; }

        public IStringMatcher AuthorizationMatcher { get; set; }

        public bool? AllowPartialMapping { get; set; }

        public ConcurrentDictionary<Guid, IMapping> Mappings { get; } = new ConcurrentDictionary<Guid, IMapping>();

        public ConcurrentDictionary<string, ScenarioState> Scenarios { get; } = new ConcurrentDictionary<string, ScenarioState>();

        public ConcurrentObservableCollection<LogEntry> LogEntries { get; } = new ConcurrentObservableCollection<LogEntry>();

        public int? RequestLogExpirationDuration { get; set; }

        public int? MaxRequestLogCount { get; set; }

        public Action<IAppBuilder> PreWireMockMiddlewareInit { get; set; }

        public Action<IAppBuilder> PostWireMockMiddlewareInit { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.FileSystemHandler"/>
        public IFileSystemHandler FileSystemHandler { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.AllowBodyForAllHttpMethods"/>
        public bool? AllowBodyForAllHttpMethods { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.AllowOnlyDefinedHttpStatusCodeInResponse"/>
        public bool? AllowOnlyDefinedHttpStatusCodeInResponse { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.DisableResponseBodyParsing"/>
        public bool? DisableJsonBodyParsing { get; set; }
    }
}