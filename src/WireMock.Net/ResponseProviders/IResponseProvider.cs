using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WireMock.ResponseProviders
{
    /// <summary>
    /// The Response Provider interface.
    /// </summary>
    public interface IResponseProvider
    {
        /// <summary>
        /// The provide response.
        /// </summary>
        /// <param name="requestMessage">The request.</param>
        /// <returns>The <see cref="ResponseMessage"/>.</returns>
        Task<ResponseMessage> ProvideResponseAsync([NotNull] RequestMessage requestMessage);
    }
}