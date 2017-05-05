using JetBrains.Annotations;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The ProxyResponseBuilder interface.
    /// </summary>
    public interface IProxyResponseBuilder : IStatusCodeResponseBuilder
    {
        /// <summary>
        /// From Proxy URL.
        /// </summary>
        /// <param name="proxyUrl">The proxy url.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder FromProxyUrl([NotNull] string proxyUrl);
    }
}