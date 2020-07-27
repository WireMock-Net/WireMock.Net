using System;

namespace WireMock.Settings
{
    /// <summary>
    /// IFluentMockServerSettings
    /// </summary>
    [Obsolete("Use IWireMockServerSettings. This will removed in next version (1.3.x)")]
    public interface IFluentMockServerSettings : IWireMockServerSettings
    {
    }
}