using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Extensions;
using WireMock.Validation;

namespace WireMock
{
    /// <summary>
    /// The request.
    /// </summary>
    public class RequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        /// <param name="url">The original url.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="bodyAsBytes">The bodyAsBytes byte[].</param>
        /// <param name="body">The body string.</param>
        /// <param name="headers">The headers.</param>
        public RequestMessage([NotNull] Uri url, [NotNull] string verb, [CanBeNull] byte[] bodyAsBytes, [CanBeNull] string body, [CanBeNull] IDictionary<string, string> headers = null)
        {
            Check.NotNull(url, nameof(url));
            Check.NotNull(verb, nameof(verb));

            Url = url.ToString();
            Path = url.AbsolutePath;
            Verb = verb.ToLower();
            BodyAsBytes = bodyAsBytes;
            Body = body;
            Headers = headers;

            string query = url.Query;
            if (!string.IsNullOrEmpty(query))
            {
                if (query.StartsWith("?"))
                {
                    query = query.Substring(1);
                }

                Parameters = query.Split('&').Aggregate(
                    new Dictionary<string, List<string>>(),
                    (dict, term) =>
                        {
                            var key = term.Split('=')[0];
                            if (!dict.ContainsKey(key))
                            {
                                dict.Add(key, new List<string>());
                            }

                            dict[key].Add(term.Split('=')[1]);
                            return dict;
                        });

                var tmpDictionary = new Dictionary<string, object>();
                foreach (var parameter in Parameters.Where(p => p.Value.Any()))
                {
                    if (parameter.Value.Count == 1)
                    {
                        tmpDictionary.Add(parameter.Key, parameter.Value.First());
                    }
                    else
                    {
                        tmpDictionary.Add(parameter.Key, parameter.Value);
                    }
                }
                Query = tmpDictionary.ToExpandoObject();
            }
        }

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
        /// Gets the query parameters.
        /// </summary>
        public IDictionary<string, List<string>> Parameters { get; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Gets the query as object.
        /// </summary>
        [PublicAPI]
        public dynamic Query { get; }

        /// <summary>
        /// Gets the bodyAsBytes.
        /// </summary>
        public byte[] BodyAsBytes { get; }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// The get parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The parameter.s</returns>
        public List<string> GetParameter(string key)
        {
            return Parameters.ContainsKey(key) ? Parameters[key] : new List<string>();
        }
    }
}