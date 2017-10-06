﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using JetBrains.Annotations;
using WireMock.Logging;
using WireMock.Matchers.Request;
using System.Linq;

namespace WireMock.Server
{
    public partial class FluentMockServer
    {
        /// <summary>
        /// Log entries notification handler
        /// </summary>
        [PublicAPI]
        public event NotifyCollectionChangedEventHandler LogEntriesChanged
        {
            add
            {
                lock (((ICollection) _options.LogEntries).SyncRoot)
                {
                    _options.LogEntries.CollectionChanged += value;
                }
            }
            remove
            {
                lock (((ICollection)_options.LogEntries).SyncRoot)
                {
                    _options.LogEntries.CollectionChanged -= value;
                }
            }
        }

        /// <summary>
        /// Gets the request logs.
        /// </summary>
        [PublicAPI]
        public IEnumerable<LogEntry> LogEntries
        {
            get
            {
                lock (((ICollection)_options.LogEntries).SyncRoot)
                {
                    return new ReadOnlyCollection<LogEntry>(_options.LogEntries);
                }
            }
        }

        /// <summary>
        /// The search log-entries based on matchers.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        [PublicAPI]
        public IEnumerable<LogEntry> FindLogEntries([NotNull] params IRequestMatcher[] matchers)
        {
            lock (((ICollection)_options.LogEntries).SyncRoot)
            {
                var results = new Dictionary<LogEntry, RequestMatchResult>();

                foreach (var log in _options.LogEntries)
                {
                    var requestMatchResult = new RequestMatchResult();
                    foreach (var matcher in matchers)
                    {
                        matcher.GetMatchingScore(log.RequestMessage, requestMatchResult);
                    }

                    if (requestMatchResult.AverageTotalScore > 0.99)
                        results.Add(log, requestMatchResult);
                }

                return new ReadOnlyCollection<LogEntry>(results.OrderBy(x => x.Value).Select(x => x.Key).ToList());
            }
        }

        /// <summary>
        /// Resets the LogEntries.
        /// </summary>
        [PublicAPI]
        public void ResetLogEntries()
        {
            lock (((ICollection)_options.LogEntries).SyncRoot)
            {
                _options.LogEntries.Clear();
            }
        }

        /// <summary>
        /// Deletes the mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        [PublicAPI]
        public bool DeleteLogEntry(Guid guid)
        {
            lock (((ICollection)_options.LogEntries).SyncRoot)
            {
                // Check a logentry exists with the same GUID, if so, remove it.
                var existing = _options.LogEntries.FirstOrDefault(m => m.Guid == guid);
                if (existing != null)
                {
                    _options.LogEntries.Remove(existing);
                    return true;
                }

                return false;
            }
        }
    }
}
