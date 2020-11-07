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
    internal interface IWireMockMiddlewareOptions
    {
        IWireMockLogger Logger { get; set; }

        TimeSpan? RequestProcessingDelay { get; set; }

        IStringMatcher AuthorizationMatcher { get; set; }

        bool? AllowPartialMapping { get; set; }

        ConcurrentDictionary<Guid, IMapping> Mappings { get; }

        ConcurrentDictionary<string, ScenarioState> Scenarios { get; }

        ConcurrentObservableCollection<LogEntry> LogEntries { get; }

        int? RequestLogExpirationDuration { get; set; }

        int? MaxRequestLogCount { get; set; }

        Action<IAppBuilder> PreWireMockMiddlewareInit { get; set; }

        Action<IAppBuilder> PostWireMockMiddlewareInit { get; set; }

        IFileSystemHandler FileSystemHandler { get; set; }

        bool? AllowBodyForAllHttpMethods { get; set; }

        bool? AllowOnlyDefinedHttpStatusCodeInResponse { get; set; }

        bool? DisableJsonBodyParsing { get; set; }

        bool? DisableRequestBodyDecompressing { get; set; }

        bool? HandleRequestsSynchronously { get; set; }

        string X509StoreName { get; set; }

        string X509StoreLocation { get; set; }

        string X509CertificateFilePath { get; set; }

        string X509CertificatePassword { get; set; }

        bool CustomCertificateDefined { get; }
    }
}