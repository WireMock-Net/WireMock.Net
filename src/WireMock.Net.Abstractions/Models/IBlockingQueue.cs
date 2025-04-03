// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;

namespace WireMock.Models;

/// <summary>
/// A simple implementation for a Blocking Queue.
/// </summary>
/// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
public interface IBlockingQueue<T>
{
    /// <summary>
    /// Writes an item to the queue and signals that an item is available.
    /// </summary>
    /// <param name="item">The item to be added to the queue.</param>
    void Write(T item);

    /// <summary>
    /// Tries to read an item from the queue. Waits until an item is available or the timeout occurs.
    /// </summary>
    /// <param name="item">The item read from the queue, or default if the timeout occurs.</param>
    /// <returns>True if an item was successfully read; otherwise, false.</returns>
    bool TryRead([NotNullWhen(true)] out T? item);

    /// <summary>
    /// Closes the queue and signals all waiting threads.
    /// </summary>
    public void Close();
}