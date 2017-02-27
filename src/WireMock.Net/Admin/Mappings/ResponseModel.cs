using System.Collections.Generic;

namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// ResponseModel
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// Gets or sets the HTTP status.
        /// </summary>
        /// <value>
        /// The HTTP status.
        /// </value>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string BodyAsBase64 { get; set; }

        /// <summary>
        /// Gets or sets the body (as JSON object).
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public object BodyAsJson { get; set; }

        /// <summary>
        /// Gets or sets the body encoding.
        /// </summary>
        /// <value>
        /// The body encoding.
        /// </value>
        public EncodingModel BodyEncoding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use transformer].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use transformer]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTransformer { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds.
        /// </summary>
        /// <value>
        /// The delay in milliseconds.
        /// </value>
        public int? Delay { get; set; }
    }
}