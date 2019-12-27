using JetBrains.Annotations;
using System.Threading.Tasks;
using WireMock.Settings;

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
        /// <param name="settings">The WireMockServerSettings.</param>
        /// <returns>The <see cref="ResponseMessage"/>.</returns>
        Task<ResponseMessage> ProvideResponseAsync([NotNull] RequestMessage requestMessage, [NotNull] IWireMockServerSettings settings);
    }
}