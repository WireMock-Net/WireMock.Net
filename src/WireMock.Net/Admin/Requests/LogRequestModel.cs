using System;
using System.Collections.Generic;
using WireMock.Admin.Mappings;
using WireMock.Util;

namespace WireMock.Admin.Requests
{
    /// <summary>
    /// RequestMessage Model
    /// </summary>
    public class LogRequestModel
    {
        /// <summary>
        /// The Client IP Address.
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// The DateTime.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The Path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The Absolute Path.
        /// </summary>
        public string AbsolutePath { get; set; }

        /// <summary>
        /// Gets the url (relative).
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The absolete URL.
        /// </summary>
        public string AbsoluteUrl { get; set; }

        /// <summary>
        /// The query.
        /// </summary>
        public IDictionary<string, WireMockList<string>> Query { get; set; }

        /// <summary>
        /// The method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The Headers.
        /// </summary>
        public IDictionary<string, WireMockList<string>> Headers { get; set; }

        /// <summary>
        /// Tthe Cookies.
        /// </summary>
        public IDictionary<string, string> Cookies { get; set; }

        /// <summary>
        /// The body (as string).
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The body (as JSON object).
        /// </summary>
        public object BodyAsJson { get; set; }

        /// <summary>
        /// The body (as bytearray).
        /// </summary>
        public byte[] BodyAsBytes { get; set; }

        /// <summary>
        /// The body encoding.
        /// </summary>
        public EncodingModel BodyEncoding { get; set; }
    }
}