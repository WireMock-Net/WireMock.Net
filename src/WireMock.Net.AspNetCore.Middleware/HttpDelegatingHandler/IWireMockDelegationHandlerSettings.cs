// Copyright © WireMock.Net

namespace WireMock.Net.AspNetCore.Middleware.HttpDelegatingHandler;

internal interface IWireMockDelegationHandlerSettings
{
    bool AlwaysRedirect { get; set; }
}