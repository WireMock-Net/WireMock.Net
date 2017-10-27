using System.Collections.Generic;
using WireMock.Admin.Mappings;

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
        public IDictionary<string, string[]> Headers { get; set; }

        /// <summary>
        /// Gets or sets the body destination (SameAsSource, String or Bytes).
        /// </summary>
        public string BodyDestination { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the body as bytes.
        /// </summary>
        public byte[] BodyAsBytes { get; set; }

        /// <summary>
        /// Gets or sets the body as file.
        /// </summary>
        public string BodyAsFile { get; set; }

        /// <summary>
        /// Is the body as file cached?
        /// </summary>
        public bool? BodyAsFileIsCached { get; set; }

        /// <summary>
        /// Gets or sets the original body.
        /// </summary>
        public string BodyOriginal { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public EncodingModel BodyEncoding { get; set; }
    }
}