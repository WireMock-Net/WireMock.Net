using System;
using System.Collections.Generic;
using WireMock.Util;

namespace WireMock.Admin.Requests
{
    /// <summary>
    /// RequestMessage Model
    /// </summary>
    public class LogRequestModel
    {
        /// <summary>
        /// Gets the DateTime.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the absolete URL.
        /// </summary>
        /// <value>
        /// The absolete URL.
        /// </value>
        public string AbsoleteUrl { get; set; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        public IDictionary<string, WireMockList<string>> Query { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the Headers.
        /// </summary>
        /// <value>
        /// The Headers.
        /// </value>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the Cookies.
        /// </summary>
        /// <value>
        /// The Cookies.
        /// </value>
        public IDictionary<string, string> Cookies { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }

        /*
        "id" : "45760a03-eebb-4387-ad0d-bb89b5d3d662",
        "request" : {
            "url" : "/received-request/9",
            "absoluteUrl" : "http://localhost:56715/received-request/9",
            "method" : "GET",
            "clientIp" : "127.0.0.1",
            "headers" : {
                "Connection" : "keep-alive",
                "Host" : "localhost:56715",
                "User-Agent" : "Apache-HttpClient/4.5.1 (Java/1.7.0_51)"
            },
            "cookies" : { },
            "browserProxyRequest" : false,
            "loggedDate" : 1471442494809,
            "bodyAsBase64" : "",
            "body" : "",
            "loggedDateString" : "2016-08-17T14:01:34Z"
        },
        "responseDefinition" : {
            "status" : 404,
            "transformers" : [ ],
            "fromConfiguredStub" : false,
            "transformerParameters" : { }
        }*/
    }
}