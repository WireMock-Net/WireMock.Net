using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WireMock.Extensions;

[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]

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
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="verb">
        /// The verb.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="headers">
        /// The headers.
        /// </param>
        public RequestMessage(string path, string query, string verb, string body, IDictionary<string, string> headers = null)
        {
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

            Path = path;
            Headers = headers;
            Verb = verb.ToLower();
            Body = body;
        }

        /// <summary>
        /// Gets the url.
        /// </summary>
        public string Url
        {
            get
            {
                if (!Parameters.Any())
                {
                    return Path;
                }

                return Path + "?" + string.Join("&", Parameters.SelectMany(kv => kv.Value.Select(value => kv.Key + "=" + value)));
            }
        }

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
        public dynamic Query { get; }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// The get parameter.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The parameter.
        /// </returns>
        public List<string> GetParameter(string key)
        {
            return Parameters.ContainsKey(key) ? Parameters[key] : new List<string>();
        }
    }
}