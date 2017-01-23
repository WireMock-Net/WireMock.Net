using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock
{
    /// <summary>
    /// The request.
    /// </summary>
    public class RequestMessage
    {
        /// <summary>
        /// Gets the url.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the verb.
        /// </summary>
        public string Verb { get; }

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
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        /// <param name="url">The original url.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="bodyAsBytes">The bodyAsBytes byte[].</param>
        /// <param name="body">The body string.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="cookies">The cookies.</param>
        public RequestMessage([NotNull] Uri url, [NotNull] string verb, [CanBeNull] byte[] bodyAsBytes, [CanBeNull] string body, [CanBeNull] IDictionary<string, string> headers = null, [CanBeNull] IDictionary<string, string> cookies = null)
        {
            Check.NotNull(url, nameof(url));
            Check.NotNull(verb, nameof(verb));

            Url = url.ToString();
            Path = url.AbsolutePath;
            Verb = verb.ToLower();
            BodyAsBytes = bodyAsBytes;
            Body = body;
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
                            var key = term.Split('=')[0];
                            if (!dict.ContainsKey(key))
                            {
                                dict.Add(key, new WireMockList<string>());
                            }

                            dict[key].Add(term.Split('=')[1]);
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
            return Query.ContainsKey(key) ? Query[key] : new WireMockList<string>();
        }
    }
}