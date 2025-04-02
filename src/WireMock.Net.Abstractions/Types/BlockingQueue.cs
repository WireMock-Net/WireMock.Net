// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace WireMock.Types;

public class BlockingQueue<T>
{
    private readonly Queue<T?> _queue = new();
    private readonly object _lockObject = new();

    public void Write(T item)
    {
        lock (_lockObject)
        {
            _queue.Enqueue(item);

            // Signal that an item is available
            Monitor.Pulse(_lockObject);
        }
    }

    public void Flush()
    {
        lock (_lockObject)
        {
            _queue.Enqueue(default);

            // Signal that an item is available
            Monitor.Pulse(_lockObject);
        }
    }

    public bool TryRead([NotNullWhen(true)] out T? item, TimeSpan timeout)
    {
        lock (_lockObject)
        {
            // Wait until an item is available or timeout occurs
            if (_queue.Count == 0)
            {
                // Return false immediately if no wait requested
                if (timeout == TimeSpan.Zero)
                {
                    item = default;
                    return false;
                }

                // Wait with timeout
                if (!Monitor.Wait(_lockObject, timeout))
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