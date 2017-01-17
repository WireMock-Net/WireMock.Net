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
// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace WireMock
{
    /// <summary>
    /// The composite request spec.
    /// </summary>
    public class CompositeRequestSpec : ISpecifyRequests
    {
        /// <summary>
        /// The _request specs.
        /// </summary>
        private readonly IEnumerable<ISpecifyRequests> _requestSpecs;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeRequestSpec"/> class. 
        /// The constructor.
        /// </summary>
        /// <param name="requestSpecs">
        /// The <see cref="IEnumerable&lt;ISpecifyRequests&gt;"/> request specs.
        /// </param>
        public CompositeRequestSpec(IEnumerable<ISpecifyRequests> requestSpecs)
        {
            _requestSpecs = requestSpecs;
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
            return _requestSpecs.All(spec => spec.IsSatisfiedBy(request));
        }
    }
}
