// Copyright © WireMock.Net

namespace WireMock.Org.Abstractions
{
    public class WireMockOrgResponse
    {
        /// <summary>
        /// The HTTP status code to be returned
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// The HTTP status message to be returned
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Map of response headers to send
        /// </summary>
        public object Headers { get; set; }

        /// <summary>
        /// Extra request headers to send when proxying to another host.
        /// </summary>
        public object AdditionalProxyRequestHeaders { get; set; }

        /// <summary>
        /// The response body as a string. Only one of body, base64Body, jsonBody or bodyFileName may be specified.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The response body as a base64 encoded string (useful for binary content). Only one of body, base64Body, jsonBody or bodyFileName may be specified.
        /// </summary>
        public string Base64Body { get; set; }

        /// <summary>
        /// The response body as a JSON object. Only one of body, base64Body, jsonBody or bodyFileName may be specified.
        /// </summary>
        public object JsonBody { get; set; }

        /// <summary>
        /// The path to the file containing the response body, relative to the configured file root. Only one of body, base64Body, jsonBody or bodyFileName may be specified.
        /// </summary>
        public string BodyFileName { get; set; }

        /// <summary>
        /// The fault to apply (instead of a full, valid response).
        /// </summary>
        public string Fault { get; set; }

        /// <summary>
        /// Number of milliseconds to delay be before sending the response.
        /// </summary>
        public int FixedDelayMilliseconds { get; set; }

        /// <summary>
        /// The delay distribution. Valid property configuration is either median/sigma/type or lower/type/upper.
        /// </summary>
        public object DelayDistribution { get; set; }

        /// <summary>
        /// Read-only flag indicating false if this was the default, unmatched response. Not present otherwise.
        /// </summary>
        public bool? FromConfiguredStub { get; set; }

        /// <summary>
        /// The base URL of the target to proxy matching requests to.
        /// </summary>
        public string ProxyBaseUrl { get; set; }

        /// <summary>
        /// Parameters to apply to response transformers.
        /// </summary>
        public object TransformerParameters { get; set; }

        /// <summary>
        /// List of names of transformers to apply to this response.
        /// </summary>
        public string[] Transformers { get; set; }
    }
}