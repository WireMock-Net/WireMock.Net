using System;
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

namespace WireMock
{
    /// <summary>
    /// The request path spec.
    /// </summary>
    public class RequestPathSpec : ISpecifyRequests
    {
        /// <summary>
        /// The pathRegex.
        /// </summary>
        private readonly Regex pathRegex;

        /// <summary>
        /// The url function
        /// </summary>
        private readonly Func<string, bool> pathFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestPathSpec"/> class.
        /// </summary>
        /// <param name="path">
        /// The path Regex pattern.
        /// </param>
        public RequestPathSpec([NotNull, RegexPattern] string path)
        {
            Check.NotNull(path, nameof(path));
            pathRegex = new Regex(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestPathSpec"/> class.
        /// </summary>
        /// <param name="func">
        /// The url func.
        /// </param>
        public RequestPathSpec([NotNull] Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));
            pathFunc = func;
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
            return pathRegex?.IsMatch(requestMessage.Path) ?? pathFunc(requestMessage.Path);
        }
    }
}