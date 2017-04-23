using System;
using System.Collections.Generic;
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

        public IList<LogEntry> LogEntries { get; set; }

        public WireMockMiddlewareOptions()
        {
            Mappings = new List<Mapping>();
            LogEntries = new List<LogEntry>();
        }
    }
}