using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using JetBrains.Annotations;
using WireMock.Admin.Mappings;
using WireMock.Logging;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server interface.
    /// </summary>
    public interface IWireMockServer : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this server is started.
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// Gets the request logs.
        /// </summary>
        IEnumerable<ILogEntry> LogEntries { get; }

        /// <summary>
        /// Gets the mappings as MappingModels.
        /// </summary>
        IEnumerable<MappingModel> MappingModels { get; }

        // <summary>
        // Gets the mappings.
        // </summary>
        //[PublicAPI]
        //IEnumerable<IMapping> Mappings { get; }

        /// <summary>
        /// Gets the ports.
        /// </summary>
        List<int> Ports { get; }

        /// <summary>
        /// Gets the urls.
        /// </summary>
        string[] Urls { get; }

        //ConcurrentDictionary<string, ScenarioState> Scenarios { get; }

        /// <summary>
        /// Occurs when [log entries changed].
        /// </summary>
        event NotifyCollectionChangedEventHandler LogEntriesChanged;

        /// <summary>
        /// Adds the catch all mapping.
        /// </summary>
        void AddCatchAllMapping();

        /// <summary>
        /// The add request processing delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        void AddGlobalProcessingDelay(TimeSpan delay);

        /// <summary>
        /// Set the partial mapping to allowed (if true, you can also provide 'enforceHttpMethod').
        /// </summary>
        void AllowPartialMapping(bool allow = true, bool enforceHttpMethod = false);

        /// <summary>
        /// Deletes a LogEntry.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        bool DeleteLogEntry(Guid guid);

        /// <summary>
        /// Deletes the mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        bool DeleteMapping(Guid guid);

        //IEnumerable<LogEntry> FindLogEntries([NotNull] params IRequestMatcher[] matchers);

        //IRespondWithAProvider Given(IRequestMatcher requestMatcher, bool saveToFile = false);

        /// <summary>
        /// Reads a static mapping file and adds or updates the mapping.
        /// </summary>
        /// <param name="path">The path.</param>
        bool ReadStaticMappingAndAddOrUpdate([NotNull] string path);

        /// <summary>
        /// Reads the static mappings from a folder.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
        void ReadStaticMappings([CanBeNull] string folder = null);

        /// <summary>
        /// Removes the basic authentication.
        /// </summary>
        void RemoveBasicAuthentication();

        /// <summary>
        /// Resets LogEntries and Mappings.
        /// </summary>
        void Reset();

        /// <summary>
        /// Resets the Mappings.
        /// </summary>
        void ResetMappings();

        /// <summary>
        /// Resets all Scenarios.
        /// </summary>
        void ResetScenarios();

        /// <summary>
        /// Resets the LogEntries.
        /// </summary>
        void ResetLogEntries();

        /// <summary>
        /// Saves the static mappings.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
        void SaveStaticMappings([CanBeNull] string folder = null);

        /// <summary>
        /// Sets the basic authentication.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        void SetBasicAuthentication([NotNull] string username, [NotNull] string password);

        /// <summary>
        /// Sets the maximum RequestLog count.
        /// </summary>
        /// <param name="maxRequestLogCount">The maximum RequestLog count.</param>
        void SetMaxRequestLogCount([CanBeNull] int? maxRequestLogCount);

        /// <summary>
        /// Sets RequestLog expiration in hours.
        /// </summary>
        /// <param name="requestLogExpirationDuration">The RequestLog expiration in hours.</param>
        void SetRequestLogExpirationDuration([CanBeNull] int? requestLogExpirationDuration);

        /// <summary>
        /// Stop this server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Watches the static mappings for changes.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
        void WatchStaticMappings([CanBeNull] string folder = null);

        /// <summary>
        /// Register the mappings (via <see cref="MappingModel"/>).
        /// </summary>
        /// <param name="mappings">The MappingModels</param>
        IWireMockServer WithMapping(params MappingModel[] mappings);

        /// <summary>
        /// Register the mappings (via json string).
        /// </summary>
        /// <param name="mappings">The mapping(s) as json string.</param>
        IWireMockServer WithMapping(string mappings);
    }
}