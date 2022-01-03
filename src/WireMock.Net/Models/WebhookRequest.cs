using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Models
{
    /// <summary>
    /// WebhookRequest
    /// </summary>
    public class WebhookRequest : IWebhookRequest
    {
        /// <inheritdoc cref="IWebhookRequest.Url"/>
        public string Url { get; set; }

        /// <inheritdoc cref="IWebhookRequest.Method"/>
        public string Method { get; set; }

        /// <inheritdoc cref="IWebhookRequest.Headers"/>
        public IDictionary<string, WireMockList<string>> Headers { get; set; }

        /// <inheritdoc cref="IWebhookRequest.BodyData"/>
        public IBodyData BodyData { get; set; }

        /// <inheritdoc cref="IWebhookRequest.UseTransformer"/>
        public bool? UseTransformer { get; set; }

        /// <inheritdoc cref="IWebhookRequest.TransformerType"/>
        public TransformerType TransformerType { get; set; }

        /// <inheritdoc cref="IWebhookRequest.TransformerReplaceNodeOption"/>
        public ReplaceNodeOption TransformerReplaceNodeOption { get; set; }
    }
}