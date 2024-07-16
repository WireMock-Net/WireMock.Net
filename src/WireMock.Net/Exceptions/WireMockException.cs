// Copyright © WireMock.Net

using System;

namespace WireMock.Exceptions;

/// <summary>
/// WireMockException
/// </summary>
/// <seealso cref="Exception" />
public class WireMockException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockException"/> class.
    /// </summary>
    public WireMockException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public WireMockException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    public WireMockException(string message, Exception inner) : base(message, inner)
    {
    }
}