using System;
using System.Collections.Concurrent;
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

        /// <inheritdoc cref="IWireMockMiddlewareOptions.DisableJsonBodyParsing"/>
        public bool? DisableJsonBodyParsing { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.DisableRequestBodyDecompressing"/>
        public bool? DisableRequestBodyDecompressing { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.HandleRequestsSynchronously"/>
        public bool? HandleRequestsSynchronously { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.X509StoreName"/>
        public string X509StoreName { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.X509StoreLocation"/>
        public string X509StoreLocation { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.X509ThumbprintOrSubjectName"/>
        public string X509ThumbprintOrSubjectName { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.X509CertificateFilePath"/>
        public string X509CertificateFilePath { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.X509CertificatePassword"/>
        public string X509CertificatePassword { get; set; }

        /// <inheritdoc cref="IWireMockMiddlewareOptions.CustomCertificateDefined"/>
        public bool CustomCertificateDefined =>
            !string.IsNullOrEmpty(X509StoreName) && !string.IsNullOrEmpty(X509StoreLocation) ||
            !string.IsNullOrEmpty(X509CertificateFilePath) && !string.IsNullOrEmpty(X509CertificatePassword);
    }
}