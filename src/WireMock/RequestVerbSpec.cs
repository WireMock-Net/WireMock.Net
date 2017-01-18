using System.Diagnostics.CodeAnalysis;
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
    /// The request verb spec.
    /// </summary>
    internal class RequestVerbSpec : ISpecifyRequests
    {
        /// <summary>
        /// The _verb.
        /// </summary>
        private readonly string _verb;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestVerbSpec"/> class.
        /// </summary>
        /// <param name="verb">
        /// The verb.
        /// </param>
        public RequestVerbSpec(string verb)
        {
            _verb = verb.ToLower();
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
        public bool IsSatisfiedBy([NotNull] RequestMessage requestMessage)
        {
            return requestMessage.Verb == _verb;
        }
    }
}
