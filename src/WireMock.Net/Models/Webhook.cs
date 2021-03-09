using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Types;

namespace WireMock.Models
{
    /// <summary>
    /// Webhook
    /// </summary>
    public class Webhook : IWebhook
    {
        /// <inheritdoc cref="IWebhook.Request"/>
        public IWebhookRequest Request { get; set; }
    }
}