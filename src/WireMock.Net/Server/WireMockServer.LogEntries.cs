// Copyright © WireMock.Net

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.Server;

public partial class WireMockServer
{
    /// <inheritdoc />
    [PublicAPI]
    public event NotifyCollectionChangedEventHandler LogEntriesChanged
    {
        add => _logEntriesChanged += value;
        remove => _logEntriesChanged -= value;
    }

    /// <inheritdoc cref="IWireMockServer.LogEntries" />
    [PublicAPI]
    public IEnumerable<ILogEntry> LogEntries => new ReadOnlyCollection<LogEntry>(_options.LogEntries.ToList());

    /// <summary>
    /// The search log-entries based on matchers.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IEnumerable"/>.</returns>
    [PublicAPI]
    public IEnumerable<LogEntry> FindLogEntries(params IRequestMatcher[] matchers)
    {
        Guard.NotNull(matchers);

        var results = new Dictionary<LogEntry, RequestMatchResult>();

        foreach (var log in _options.LogEntries.ToList())
        {
            var requestMatchResult = new RequestMatchResult();
            foreach (var matcher in matchers)
            {
                matcher.GetMatchingScore(log.RequestMessage, requestMatchResult);
            }

            if (requestMatchResult.AverageTotalScore > MatchScores.AlmostPerfect)
            {
                results.Add(log, requestMatchResult);
            }
        }

        return new ReadOnlyCollection<LogEntry>(results.OrderBy(x => x.Value).Select(x => x.Key).ToList());
    }

    /// <inheritdoc cref="IWireMockServer.ResetLogEntries" />
    [PublicAPI]
    public void ResetLogEntries()
    {
        _options.LogEntries.Clear();
    }

    /// <inheritdoc cref="IWireMockServer.DeleteLogEntry" />
    [PublicAPI]
    public bool DeleteLogEntry(Guid guid)
    {
        // Check a LogEntry exists with the same GUID, if so, remove it.
        var existing = _options.LogEntries.ToList().FirstOrDefault(m => m.Guid == guid);
        if (existing != null)
        {
            _options.LogEntries.Remove(existing);
            return true;
        }

        return false;
    }

    private NotifyCollectionChangedEventHandler? _logEntriesChanged;

    private void LogEntries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_logEntriesChanged is { })
        {
            foreach (var handler in _logEntriesChanged.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(this, e);
                }
                catch (Exception exception)
                {
                    _options.Logger.Error("Error calling the LogEntriesChanged event handler: {0}", exception.Message);
                }
            }
        }
    }
}