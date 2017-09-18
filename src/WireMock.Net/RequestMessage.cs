using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Util;
using WireMock.Validation;
using System.Text;

namespace WireMock
{
    /// <summary>
    /// The request.
    /// </summary>
    public class RequestMessage
    {
        /// <summary>
        /// Gets the Client IP Address.
        /// </summary>
        public string ClientIP { get; }

        /// <summary>
        /// Gets the url.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the DateTime.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the method.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the cookies.
        /// </summary>
        public IDictionary<string, string> Cookies { get; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        public IDictionary<string, WireMockList<string>> Query { get; } = new Dictionary<string, WireMockList<string>>();

        /// <summary>
        /// Gets the bodyAsBytes.
        /// </summary>
        public byte[] BodyAsBytes { get; }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Gets the body encoding.
        /// </summary>
        public Encoding BodyEncoding { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        /// <param name="url">The original url.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="clientIP">The client IP Address.</param>
        /// <param name="bodyAsBytes">The bodyAsBytes byte[].</param>
        /// <param name="body">The body string.</param>
        /// <param name="bodyEncoding">The body encoding</param>
        /// <param name="headers">The headers.</param>
        /// <param name="cookies">The cookies.</param>
        public RequestMessage([NotNull] Uri url, [NotNull] string method, [NotNull] string clientIP, [CanBeNull] byte[] bodyAsBytes = null, [CanBeNull] string body = null, [CanBeNull] Encoding bodyEncoding = null, [CanBeNull] IDictionary<string, string> headers = null, [CanBeNull] IDictionary<string, string> cookies = null)
        {
            Check.NotNull(url, nameof(url));
            Check.NotNull(method, nameof(method));
            Check.NotNull(clientIP, nameof(clientIP));

            Url = url.ToString();
            Path = url.AbsolutePath;
            Method = method.ToLower();
            ClientIP = clientIP;
            BodyAsBytes = bodyAsBytes;
            Body = body;
            BodyEncoding = bodyEncoding;
            Headers = headers;
            Cookies = cookies;

            string query = url.Query;
            if (!string.IsNullOrEmpty(query))
            {
                if (query.StartsWith("?"))
                {
                    query = query.Substring(1);
                }

                Query = query.Split('&').Aggregate(
                    new Dictionary<string, WireMockList<string>>(),
                    (dict, term) =>
                    {
                        var parts = term.Split('=');
                        string key = parts[0];
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add(key, new WireMockList<string>());
                        }

                        if (parts.Length == 2)
                        {
                            dict[key].Add(parts[1]);
                        }

                        return dict;
                    });
            }
        }

        /// <summary>
        /// The get a query parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The query parameter.</returns>
        public List<string> GetParameter(string key)
        {
            return Query.ContainsKey(key) ? Query[key] : null;
        }
    }
}