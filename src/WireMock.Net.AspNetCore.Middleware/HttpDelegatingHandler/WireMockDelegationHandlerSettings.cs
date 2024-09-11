// Copyright © WireMock.Net

namespace WireMock.Net.AspNetCore.Middleware.HttpDelegatingHandler;

internal class WireMockDelegationHandlerSettings : IWireMockDelegationHandlerSettings
{
    public bool AlwaysRedirect { get; set; }
}