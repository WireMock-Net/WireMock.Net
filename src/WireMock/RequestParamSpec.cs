using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

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
    /// The request parameters spec.
    /// </summary>
    public class RequestParamSpec : ISpecifyRequests
    {
        /// <summary>
        /// The _key.
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// The _values.
        /// </summary>
        private readonly List<string> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestParamSpec"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public RequestParamSpec(string key, List<string> values)
        {
            _key = key;
            _values = values;
        }

        /// <summary>
        /// The is satisfied by.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSatisfiedBy([NotNull] Request request)
        {
            return request.GetParameter(_key).Intersect(_values).Count() == _values.Count;
        }
    }
}
