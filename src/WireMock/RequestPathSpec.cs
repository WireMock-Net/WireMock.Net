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
    /// The request path spec.
    /// </summary>
    public class RequestPathSpec : ISpecifyRequests
    {
        /// <summary>
        /// The _path.
        /// </summary>
        private readonly Regex _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestPathSpec"/> class.
        /// </summary>
        /// <param name="path">
        /// The path Regex pattern.
        /// </param>
        public RequestPathSpec([NotNull, RegexPattern] string path)
        {
            Check.NotNull(path, nameof(path));
            _path = new Regex(path);
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
            return _path.IsMatch(request.Path);
        }
    }
}
