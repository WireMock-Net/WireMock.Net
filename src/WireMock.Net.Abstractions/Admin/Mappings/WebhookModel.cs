// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// The Webhook
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class WebhookModel
{
    /// <summary>
    /// The Webhook Request.
    /// </summary>
    public WebhookRequestModel Request { get; set; } = null!;
}