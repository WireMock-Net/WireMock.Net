//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using JetBrains.Annotations;
//using WireMock.Admin.Mappings;
//using WireMock.Logging;
//using WireMock.Matchers.Request;

//namespace WireMock.Server
//{
//    /// <summary>
//    /// The fluent mock server interface.
//    /// </summary>
//    public interface IWireMockServer : IDisposable
//    {
//        /// <summary>
//        /// Gets a value indicating whether this server is started.
//        /// </summary>
//        bool IsStarted { get; }

//        IEnumerable<LogEntry> LogEntries { get; }

//        IEnumerable<MappingModel> MappingModels { get; }

//        IEnumerable<IMapping> Mappings { get; }

//        List<int> Ports { get; }

//        ConcurrentDictionary<string, ScenarioState> Scenarios { get; }

//        string[] Urls { get; }

//        event NotifyCollectionChangedEventHandler LogEntriesChanged;

//        void AddCatchAllMapping();

//        void AddGlobalProcessingDelay(TimeSpan delay);

//        void AllowPartialMapping(bool allow = true);

//        bool DeleteLogEntry(Guid guid);

//        bool DeleteMapping(Guid guid);

//        IEnumerable<LogEntry> FindLogEntries([NotNull] params IRequestMatcher[] matchers);

//        IRespondWithAProvider Given(IRequestMatcher requestMatcher, bool saveToFile = false);

//        bool ReadStaticMappingAndAddOrUpdate([NotNull] string path);

//        void ReadStaticMappings([CanBeNull] string folder = null);

//        void RemoveBasicAuthentication();

//        void Reset();

//        void ResetLogEntries();

//        void ResetMappings();

//        void ResetScenarios();

//        void SaveStaticMappings([CanBeNull] string folder = null);

//        void SetBasicAuthentication([NotNull] string username, [NotNull] string password);

//        void SetMaxRequestLogCount([CanBeNull] int? maxRequestLogCount);

//        void SetRequestLogExpirationDuration([CanBeNull] int? requestLogExpirationDuration);

//        void Stop();

//        void WatchStaticMappings([CanBeNull] string folder = null);

//        void WithMapping(params MappingModel[] mappings);

//        void WithMapping(string mappings);
//    }
//}