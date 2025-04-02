// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace WireMock.Types;

/// <summary>
/// A simple implementation for a Blocking Queue.
/// </summary>
/// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
public class BlockingQueue<T>(TimeSpan? readTimeout = null)
{
    private readonly TimeSpan _readTimeout = readTimeout ?? TimeSpan.FromHours(1);
    private readonly Queue<T?> _queue = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// Writes an item to the queue and signals that an item is available.
    /// </summary>
    /// <param name="item">The item to be added to the queue.</param>
    public void Write(T item)
    {
        lock (_lockObject)
        {
            _queue.Enqueue(item);

            // Signal that an item is available
            Monitor.Pulse(_lockObject);
        }
    }

    /// <summary>
    /// Flushes the queue by adding a default item and signals that an item is available.
    /// </summary>
    public void Flush()
    {
        lock (_lockObject)
        {
            _queue.Enqueue(default);

            // Signal that an item is available
            Monitor.Pulse(_lockObject);
        }
    }

    /// <summary>
    /// Tries to read an item from the queue. Waits until an item is available or the timeout occurs.
    /// </summary>
    /// <param name="item">The item read from the queue, or default if the timeout occurs.</param>
    /// <returns>True if an item was successfully read; otherwise, false.</returns>
    public bool TryRead([NotNullWhen(true)] out T? item)
    {
        lock (_lockObject)
        {
            // Wait until an item is available or timeout occurs
            if (_queue.Count == 0)
            {
                // Wait with timeout
                if (!Monitor.Wait(_lockObject, _readTimeout))
                {
                    item = default;
                    return false;
                }
            }

            // After waiting, check if we have items
            if (_queue.Count == 0)
            {
                item = default;
                return false;
            }

            item = _queue.Dequeue();
            return item != null;
        }
    }
}