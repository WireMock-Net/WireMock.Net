using System;
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

        public IList<Mapping> Mappings { get; set; }

        public ObservableCollection<LogEntry> LogEntries { get; }
        
        public int? RequestLogExpirationDuration { get; set; }

        public int? MaxRequestLogCount { get; set; }

        public WireMockMiddlewareOptions()
        {
            Mappings = new List<Mapping>();
            LogEntries = new ObservableCollection<LogEntry>();
        }
    }
}