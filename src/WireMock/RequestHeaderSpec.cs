using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
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
// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace WireMock
{
    /// <summary>
    /// The request header spec.
    /// </summary>
    public class RequestHeaderSpec : ISpecifyRequests
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The patternRegex.
        /// </summary>
        private readonly Regex patternRegex;

        /// <summary>
        /// The header function
        /// </summary>
        private readonly Func<IDictionary<string, string>, bool> headerFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaderSpec"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="ignoreCase">The ignoreCase.</param>
        public RequestHeaderSpec([NotNull] string name, [NotNull, RegexPattern] string pattern, bool ignoreCase = true)
        {
            this.name = name;
            patternRegex = ignoreCase ? new Regex(pattern, RegexOptions.IgnoreCase) : new Regex(pattern);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaderSpec"/> class.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        public RequestHeaderSpec([NotNull] Func<IDictionary<string, string>, bool> func)
        {
            Check.NotNull(func, nameof(func));
            headerFunc = func;
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
            if (patternRegex == null)
                return headerFunc(requestMessage.Headers);

            string headerValue = requestMessage.Headers[name];
            return patternRegex.IsMatch(headerValue);
        }
    }
}