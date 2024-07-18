// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// WebProxy settings
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class WebProxyModel
{
    /// <summary>
    /// A string instance that contains the address of the proxy server.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// The user name associated with the credentials. [optional]
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The password for the user name associated with the credentials. [optional]
    /// </summary>
    public string? Password { get; set; }
}