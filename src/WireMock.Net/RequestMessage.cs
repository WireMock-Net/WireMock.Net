using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using JetBrains.Annotations;
using WireMock.Models;
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
        /// The original body as string, this is defined when Body or BodyAsJson are not null.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// The body (as JSON object).
        /// </summary>
        public object BodyAsJson { get; set; }

        /// <summary>
        /// The body (as bytearray).
        /// </summary>
        public byte[] BodyAsBytes { get; set; }

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
        /// The body encoding.
        /// </summary>
        public Encoding BodyEncoding { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        /// <param name="urlDetails">The original url details.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="clientIP">The client IP Address.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="cookies">The cookies.</param>
        public RequestMessage([NotNull] UrlDetails urlDetails, [NotNull] string method, [NotNull] string clientIP, [CanBeNull] BodyData body = null, [CanBeNull] IDictionary<string, string[]> headers = null, [CanBeNull] IDictionary<string, string> cookies = null)
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

            Body = body?.BodyAsString;
            BodyEncoding = body?.Encoding;
            BodyAsJson = body?.BodyAsJson;
            BodyAsBytes = body?.BodyAsBytes;

            Headers = headers?.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
            Cookies = cookies;
            RawQuery = WebUtility.UrlDecode(urlDetails.Url.Query);
            Query = ParseQuery(RawQuery);
        }

        private static IDictionary<string, WireMockList<string>> ParseQuery(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return null;
            }

            if (queryString.StartsWith("?"))
            {
                queryString = queryString.Substring(1);
            }

            return queryString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(new Dictionary<string, WireMockList<string>>(),
                (dict, term) =>
                {
                    string[] parts = term.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    string key = parts[0];
                    if (!dict.ContainsKey(key))
                    {
                        dict.Add(key, new WireMockList<string>());
                    }

                    if (parts.Length == 2)
                    {
                        string[] values = parts[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        dict[key].AddRange(values);
                    }

                    return dict;
                });
        }

        /// <summary>
        /// Get a query parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The query parameter.</returns>
        public WireMockList<string> GetParameter(string key)
        {
            if (Query == null)
            {
                return null;
            }

            return Query.ContainsKey(key) ? Query[key] : null;
        }
    }
}