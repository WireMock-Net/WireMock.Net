using System.Threading.Tasks;

namespace WireMock
{
    /// <summary>
    /// The Response Provider interface.
    /// </summary>
    public interface IResponseProvider
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
