using System;
using WireMock.Settings;

namespace WireMock.Server
{
    [Obsolete("Use WireMockServer. This will removed in next version (1.3.x)")]
    public class FluentMockServer : WireMockServer
    {
        public FluentMockServer(IFluentMockServerSettings settings) : base((IWireMockServerSettings) settings)
        {
        }
    }
}