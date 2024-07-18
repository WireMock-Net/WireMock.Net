// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// Webhook
/// </summary>
public class Webhook : IWebhook
{
    /// <inheritdoc />
    public IWebhookRequest Request { get; set; } = null!;
}