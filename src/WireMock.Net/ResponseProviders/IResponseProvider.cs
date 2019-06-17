using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Handlers;

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
        /// <param name="fileSystemHandler">The fileSystemHandler.</param>
        /// <returns>The <see cref="ResponseMessage"/>.</returns>
        Task<ResponseMessage> ProvideResponseAsync([NotNull] RequestMessage requestMessage, [NotNull] IFileSystemHandler fileSystemHandler);
    }
}