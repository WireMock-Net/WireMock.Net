using System.Collections.Generic;

namespace WireMock.Admin.Requests
{
    /// <summary>
    /// Response MessageModel
    /// </summary>
    public class LogResponseModel
    {
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Gets the headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the original body.
        /// </summary>
        public string BodyOriginal { get; set; }
    }
}