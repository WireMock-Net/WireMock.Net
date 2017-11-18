using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Util;
#if !NETSTANDARD
using Owin;
#else
using Microsoft.AspNetCore.Builder;
#endif

namespace WireMock.Owin
{
    internal class WireMockMiddlewareOptions
    {
        public TimeSpan? RequestProcessingDelay { get; set; }

        public IMatcher AuthorizationMatcher { get; set; }

        public bool AllowPartialMapping { get; set; }

        public IList<Mapping> Mappings { get; set; } = new List<Mapping>();

        public ObservableCollection<LogEntry> LogEntries { get; } = new ConcurentObservableCollection<LogEntry>();

        public int? RequestLogExpirationDuration { get; set; }

        public int? MaxRequestLogCount { get; set; }

        public IDictionary<string, object> Scenarios { get; } = new ConcurrentDictionary<string, object>();

#if !NETSTANDARD
        public Action<IAppBuilder> PreWireMockMiddlewareInit { get; set; }

        public Action<IAppBuilder> PostWireMockMiddlewareInit { get; set; }
#else
        public Action<IApplicationBuilder> PreWireMockMiddlewareInit { get; set; }

        public Action<IApplicationBuilder> PostWireMockMiddlewareInit { get; set; }
#endif
    }
}