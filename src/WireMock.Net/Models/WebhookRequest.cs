using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Models
{
    public class WebhookRequest : IWebhookRequest
    {
        public object Url { get; set; }

        public string Method { get; set; }

        public IDictionary<string, WireMockList<string>> Headers { get; set; }

        public IBodyData BodyData { get; set; }

        public bool? UseTransformer { get; set; }

        public TransformerType? TransformerType { get; set; }
    }
}