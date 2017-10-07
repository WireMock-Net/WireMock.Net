using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WireMock.Logging;
using WireMock.Matchers;

namespace WireMock.Owin
{
    internal class WireMockMiddlewareOptions
    {
        public TimeSpan? RequestProcessingDelay { get; set; }

        public IMatcher AuthorizationMatcher { get; set; }

        public bool AllowPartialMapping { get; set; }

        public IList<Mapping> Mappings { get; set; } = new List<Mapping>();

        public ObservableCollection<LogEntry> LogEntries { get; } = new ObservableCollection<LogEntry>();

        public int? RequestLogExpirationDuration { get; set; }

        public int? MaxRequestLogCount { get; set; }

        public IDictionary<string, object> Scenarios { get; } = new ConcurrentDictionary<string, object>();
    }
}