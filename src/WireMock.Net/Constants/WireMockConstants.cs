// Copyright © WireMock.Net

namespace WireMock.Constants;

internal static class WireMockConstants
{
    public const int AdminPriority = int.MinValue;
    public const int MinPriority = -1_000_000;
    public const int ProxyPriority = -2_000_000;

    public const string ContentTypeJson = "application/json";
    public const string ContentTypeTextPlain = "text/plain";

    public const string NoMatchingFound = "No matching mapping found";
}