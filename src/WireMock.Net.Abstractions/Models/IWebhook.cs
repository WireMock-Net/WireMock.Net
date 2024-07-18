// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// IWebhook
/// </summary>
public interface IWebhook
{
    /// <summary>
    /// Request
    /// </summary>
    IWebhookRequest Request { get; set; }
}