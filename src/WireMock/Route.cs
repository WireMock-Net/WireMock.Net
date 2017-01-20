using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WireMock.Matchers.Request;

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
    /// The route.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// The _request matcher.
        /// </summary>
        private readonly IRequestMatcher _requestSpec;

        /// <summary>
        /// The _provider.
        /// </summary>
        private readonly IProvideResponses _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class.
        /// </summary>
        /// <param name="requestSpec">
        /// The request matcher.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public Route(IRequestMatcher requestSpec, IProvideResponses provider)
        {
            _requestSpec = requestSpec;
            _provider = provider;
        }

        /// <summary>
        /// The response to.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<ResponseMessage> ResponseTo(RequestMessage requestMessage)
        {
            return _provider.ProvideResponse(requestMessage);
        }

        /// <summary>
        /// The is request handled.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsRequestHandled(RequestMessage requestMessage)
        {
            return _requestSpec.IsMatch(requestMessage);
        }
    }
}