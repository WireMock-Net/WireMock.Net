using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock
{
    /// <summary>
    /// The RequestMessage.
    /// </summary>
    public class RequestMessage
    {
        /// <summary>
        /// Gets the Client IP Address.
        /// </summary>
        public string ClientIP { get; }

        /// <summary>
        /// Gets the url (relative).
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the AbsoluteUrl.
        /// </summary>
        public string AbsoluteUrl { get; }

        /// <summary>
        /// Gets the DateTime.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets the path (relative).
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the AbsolutePath.
        /// </summary>
        public string AbsolutePath { get; }

        /// <summary>
        /// Gets the path segments.
        /// </summary>
        public string[] PathSegments { get; }

        /// <summary>
        /// Gets the absolute path segments.
        /// </summary>
        public string[] AbsolutePathSegments { get; }

        /// <summary>
        /// Gets the method.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        public IDictionary<string, WireMockList<string>> Headers { get; }

        /// <summary>
        /// Gets the cookies.
        /// </summary>
        public IDictionary<string, string> Cookies { get; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        public IDictionary<string, WireMockList<string>> Query { get; }

        /// <summary>
        /// Gets the raw query.
        /// </summary>
        public string RawQuery { get; }

        /// <summary>
        /// The body.
        /// </summary>
        public BodyData BodyData { get; }

        /// <summary>
        /// The original body as string. Convenience getter for Handlebars.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// The body (as JSON object). Convenience getter for Handlebars.
        /// </summary>
        public object BodyAsJson { get; }

        /// <summary>
        /// The body (as bytearray). Convenience getter for Handlebars.
        /// </summary>
        public byte[] BodyAsBytes { get; }

        /// <summary>
        /// The detected body type. Convenience getter for Handlebars.
        /// </summary>
        public string DetectedBodyType { get; }

        /// <summary>
        /// The detected body type from the Content-Type header. Convenience getter for Handlebars.
        /// </summary>
        public string DetectedBodyTypeFromContentType { get; }

        /// <summary>
        /// Gets the Host
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the protocol
        /// </summary>
        public string Protocol { get; }

        /// <summary>
        /// Gets the port
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the origin
        /// </summary>
        public string Origin { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        /// <param name="urlDetails">The original url details.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="clientIP">The client IP Address.</param>
        /// <param name="bodyData">The BodyData.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="cookies">The cookies.</param>
        public RequestMessage([NotNull] UrlDetails urlDetails, [NotNull] string method, [NotNull] string clientIP, [CanBeNull] BodyData bodyData = null, [CanBeNull] IDictionary<string, string[]> headers = null, [CanBeNull] IDictionary<string, string> cookies = null)
        {
            Check.NotNull(urlDetails, nameof(urlDetails));
            Check.NotNull(method, nameof(method));
            Check.NotNull(clientIP, nameof(clientIP));

            AbsoluteUrl = urlDetails.AbsoluteUrl.ToString();
            Url = urlDetails.Url.ToString();
            Protocol = urlDetails.Url.Scheme;
            Host = urlDetails.Url.Host;
            Port = urlDetails.Url.Port;
            Origin = $"{Protocol}://{Host}:{Port}";

            AbsolutePath = WebUtility.UrlDecode(urlDetails.AbsoluteUrl.AbsolutePath);
            Path = WebUtility.UrlDecode(urlDetails.Url.AbsolutePath);
            PathSegments = Path.Split('/').Skip(1).ToArray();
            AbsolutePathSegments = AbsolutePath.Split('/').Skip(1).ToArray();

            Method = method;
            ClientIP = clientIP;

            BodyData = bodyData;

            // Convenience getters for e.g. Handlebars
            Body = BodyData?.BodyAsString;
            BodyAsJson = BodyData?.BodyAsJson;
            BodyAsBytes = BodyData?.BodyAsBytes;
            DetectedBodyType = BodyData?.DetectedBodyType.ToString();
            DetectedBodyTypeFromContentType = BodyData?.DetectedBodyTypeFromContentType.ToString();

            Headers = headers?.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
            Cookies = cookies;
            RawQuery = WebUtility.UrlDecode(urlDetails.Url.Query);
            Query = QueryStringParser.Parse(RawQuery);
        }

        /// <summary>
        /// Get a query parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
        /// <returns>The query parameter.</returns>
        public WireMockList<string> GetParameter(string key, bool ignoreCase = false)
        {
            if (Query == null)
            {
                return null;
            }

            var query = !ignoreCase ? Query : new Dictionary<string, WireMockList<string>>(Query, StringComparer.OrdinalIgnoreCase);

            return query.ContainsKey(key) ? query[key] : null;
        }
    }
}