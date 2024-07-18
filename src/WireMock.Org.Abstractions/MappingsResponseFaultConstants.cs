// Copyright © WireMock.Net

namespace WireMock.Org.Abstractions;

/// <summary>
/// The fault to apply (instead of a full, valid response).
/// </summary>
public static class MappingsResponseFaultConstants
{
    public const string EMPTYRESPONSE = "EMPTY_RESPONSE";

    public const string MALFORMEDRESPONSECHUNK = "MALFORMED_RESPONSE_CHUNK";
}