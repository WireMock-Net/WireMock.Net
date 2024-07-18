// Copyright © WireMock.Net

// ReSharper disable InconsistentNaming
namespace WireMock.ResponseBuilders;

/// <summary>
/// The FaultType enumeration
/// </summary>
public enum FaultType
{
    /// <summary>
    /// No Fault
    /// </summary>
    NONE,

    /// <summary>
    /// Return a completely empty response.
    /// </summary>
    EMPTY_RESPONSE,

    /// <summary>
    /// Send a defined status header, then garbage, then close the connection.
    /// </summary>
    MALFORMED_RESPONSE_CHUNK
}