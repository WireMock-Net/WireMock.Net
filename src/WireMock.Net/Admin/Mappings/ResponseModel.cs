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
        public int? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the body destination (SameAsSource, String or Bytes).
        /// </summary>
        public string BodyDestination { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string BodyFromBase64 { get; set; }

        /// <summary>
        /// Gets or sets the body (as JSON object).
        /// </summary>
        public object BodyAsJson { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation and Newtonsoft.Json.JsonTextWriter.IndentChar settings.
        /// </summary>
        public bool? BodyAsJsonIndented { get; set; }

        /// <summary>
        /// Gets or sets the body (as bytearray).
        /// </summary>
        public byte[] BodyAsBytes { get; set; }

        /// <summary>
        /// Gets or sets the body as a file.
        /// </summary>
        public string BodyAsFile { get; set; }

        /// <summary>
        /// Is the body as file cached?
        /// </summary>
        public bool? BodyAsFileIsCached { get; set; }

        /// <summary>
        /// Gets or sets the body encoding.
        /// </summary>
        public EncodingModel BodyEncoding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use transformer].
        /// </summary>
        public bool UseTransformer { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        public IDictionary<string, object> Headers { get; set; }

        /// <summary>
        /// Gets or sets the Headers (Raw).
        /// </summary>
        public string HeadersRaw { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds.
        /// </summary>
        public int? Delay { get; set; }

        /// <summary>
        /// Gets or sets the Proxy URL.
        /// </summary>
        public string ProxyUrl { get; set; }

        /// <summary>
        /// The client X509Certificate2 Thumbprint or SubjectName to use.
        /// </summary>
        public string X509Certificate2ThumbprintOrSubjectName { get; set; }
    }
}