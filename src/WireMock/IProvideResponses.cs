using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]

namespace WireMock
{
    /// <summary>
    /// The ProvideResponses interface.
    /// </summary>
    public interface IProvideResponses
    {
        /// <summary>
        /// The provide response.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<ResponseMessage> ProvideResponse(RequestMessage requestMessage);
    }
}
