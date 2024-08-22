// Copyright © WireMock.Net

using System;

namespace WireMock.Constants;

internal static class WireMockConstants
{
    internal static readonly TimeSpan DefaultRegexTimeout = TimeSpan.FromSeconds(10);
             
    internal const int AdminPriority = int.MinValue;
    internal const int MinPriority = -1_000_000;
    internal const int ProxyPriority = -2_000_000;

    internal const string ContentTypeJson = "application/json";
    internal const string ContentTypeTextPlain = "text/plain";

    internal const string NoMatchingFound = "No matching mapping found";
} 