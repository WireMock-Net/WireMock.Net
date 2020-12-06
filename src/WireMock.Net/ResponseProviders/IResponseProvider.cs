// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using JetBrains.Annotations;
using System;
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
        /// <param name="action">The action to take on a IMapping when the response is provided.</param>
        /// <returns>The <see cref="ResponseMessage"/>.</returns>
        Task<ResponseMessage> ProvideResponseAsync([NotNull] RequestMessage requestMessage, [NotNull] IWireMockServerSettings settings, [CanBeNull] Action<IMapping> action = null);
    }
}