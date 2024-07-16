// Copyright © WireMock.Net

namespace WireMock.Admin.Settings;

/// <summary>
/// WebProxySettings
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class WebProxySettingsModel
{
    /// <summary>
    /// A string instance that contains the address of the proxy server.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// The user name associated with the credentials.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The password for the user name associated with the credentials.
    /// </summary>
    public string? Password { get; set; }
}