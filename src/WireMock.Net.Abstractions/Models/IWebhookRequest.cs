using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Models
{
    /// <summary>
    /// IWebhookRequest
    /// </summary>
    public interface IWebhookRequest
    {
        /// <summary>
        /// Gets or sets the Url.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// The method
        /// </summary>
        string Method { get; set; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        IDictionary<string, WireMockList<string>> Headers { get; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        IBodyData BodyData { get; set; }

        /// <summary>
        /// Use ResponseMessage Transformer.
        /// </summary>
        bool? UseTransformer { get; set; }

        /// <summary>
        /// The transformer type.
        /// </summary>
        TransformerType TransformerType { get; set; }
    }
}