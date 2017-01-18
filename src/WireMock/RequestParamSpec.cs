using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

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
        private readonly IEnumerable<string> _values;

        private readonly Func<IDictionary<string, List<string>>, bool> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestParamSpec"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public RequestParamSpec([NotNull] string key, [NotNull] IEnumerable<string> values)
        {
            Check.NotNull(key, nameof(key));
            Check.NotNull(values, nameof(values));

            _key = key;
            _values = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestParamSpec"/> class.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        public RequestParamSpec([NotNull] Func<IDictionary<string, List<string>>, bool> func)
        {
            Check.NotNull(func, nameof(func));
            _func = func;
        }

        /// <summary>
        /// The is satisfied by.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSatisfiedBy(RequestMessage requestMessage)
        {
            if (_func != null)
            {
                return _func(requestMessage.Parameters);
            }

            return requestMessage.GetParameter(_key).Intersect(_values).Count() == _values.Count();
        }
    }
}