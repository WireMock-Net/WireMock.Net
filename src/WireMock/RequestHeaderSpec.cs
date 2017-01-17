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
    /// The request header spec.
    /// </summary>
    public class RequestHeaderSpec : ISpecifyRequests
    {
        /// <summary>
        /// The _name.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The _pattern.
        /// </summary>
        private readonly string _pattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaderSpec"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        public RequestHeaderSpec(string name, string pattern)
        {
            _name = name.ToLower();
            _pattern = pattern.ToLower();
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
            string headerValue = request.Headers[_name];
            return WildcardPatternMatcher.MatchWildcardString(_pattern, headerValue);
        }
    }
}