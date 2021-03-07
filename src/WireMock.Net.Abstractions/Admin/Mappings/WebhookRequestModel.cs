using System.Collections.Generic;

namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// RequestModel
    /// </summary>
    public class WebhookRequestModel
    {
        /// <summary>
        /// Gets or sets the Url. (Can be a string or a UrlModel)
        /// </summary>
        public object Url { get; set; }

        /// <summary>
        /// The methods
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Use BodyMessage Transformer.
        /// </summary>
        public bool? UseTransformer { get; set; }

        /// <summary>
        /// Gets the type of the transformer.
        /// </summary>
        public string TransformerType { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public BodyModel Body { get; set; }

    }
}