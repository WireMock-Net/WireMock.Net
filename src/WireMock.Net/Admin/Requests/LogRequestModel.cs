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
        /// Gets the Client IP Address.
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// Gets the DateTime.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the Path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the absolete URL.
        /// </summary>
        public string AbsoluteUrl { get; set; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        public IDictionary<string, WireMockList<string>> Query { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the Headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the Cookies.
        /// </summary>
        public IDictionary<string, string> Cookies { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the body encoding.
        /// </summary>
        public EncodingModel BodyEncoding { get; set; }
    }
}