using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace WireMock
{
    /// <summary>
    /// The response.
    /// </summary>
    public class ResponseMessage
    {
        /// <summary>
        /// The _headers.
        /// </summary>
        private readonly IDictionary<string, string> _headers = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// The status code.
        /// </summary>
        private volatile int statusCode = 200;

        /// <summary>
        /// The body.
        /// </summary>
        private volatile string body;

        /// <summary>
        /// Gets the headers.
        /// </summary>
        public IDictionary<string, string> Headers => _headers;

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return statusCode;
            }

            set
            {
                statusCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body
        {
            get
            {
                return body;
            }

            set
            {
                body = value;
            }
        }

        /// <summary>
        /// The add header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
        }
    }
}