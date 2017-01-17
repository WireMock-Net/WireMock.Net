using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace WireMock
{
    /// <summary>
    /// The request.
    /// </summary>
    public class Request
    {
        /// <summary>
        /// The _params.
        /// </summary>
        private readonly Dictionary<string, List<string>> _params = new Dictionary<string, List<string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
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
        public Request(string path, string query, string verb, string body, IDictionary<string, string> headers)
        {
            if (!string.IsNullOrEmpty(query))
            {
                if (query.StartsWith("?"))
                {
                    query = query.Substring(1);
                }

                _params = query.Split('&').Aggregate(
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
            }

            Path = path;
            Headers = headers; //.ToDictionary(kv => kv.Key.ToLower(), kv => kv.Value.ToLower());
            Verb = verb.ToLower();
            Body = body?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Gets the url.
        /// </summary>
        public string Url
        {
            get
            {
                if (!_params.Any())
                {
                    return Path;
                }

                return Path + "?" + string.Join("&", _params.SelectMany(kv => kv.Value.Select(value => kv.Key + "=" + value)));
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
            if (_params.ContainsKey(key))
            {
                return _params[key];
            }

            return new List<string>();
        }
    }
}
