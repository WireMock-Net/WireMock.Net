using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using System.Text.RegularExpressions;
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
    /// The request url spec.
    /// </summary>
    public class RequestUrlSpec : ISpecifyRequests
    {
        /// <summary>
        /// The urlRegex.
        /// </summary>
        private readonly Regex urlRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestUrlSpec"/> class.
        /// </summary>
        /// <param name="url">
        /// The url Regex pattern.
        /// </param>
        public RequestUrlSpec([NotNull, RegexPattern] string url)
        {
            Check.NotNull(url, nameof(url));
            urlRegex = new Regex(url);
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
        public bool IsSatisfiedBy(Request request)
        {
            return urlRegex.IsMatch(request.Url);
        }
    }
}
