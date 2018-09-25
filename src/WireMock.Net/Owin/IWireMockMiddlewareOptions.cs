using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using WireMock.Logging;
using WireMock.Matchers;
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

        bool AllowPartialMapping { get; set; }

        ConcurrentDictionary<Guid, Mapping> Mappings { get; }

        ConcurrentDictionary<string, ScenarioState> Scenarios { get; }

        ObservableCollection<LogEntry> LogEntries { get; }

        int? RequestLogExpirationDuration { get; set; }

        int? MaxRequestLogCount { get; set; }

        Action<IAppBuilder> PreWireMockMiddlewareInit { get; set; }

        Action<IAppBuilder> PostWireMockMiddlewareInit { get; set; }
    }
}