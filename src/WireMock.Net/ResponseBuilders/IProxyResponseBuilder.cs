using JetBrains.Annotations;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The ProxyResponseBuilder interface.
    /// </summary>
    public interface IProxyResponseBuilder : IStatusCodeResponseBuilder
    {
        /// <summary>
        /// With Proxy URL.
        /// </summary>
        /// <param name="proxyUrl">The proxy url.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithProxy([NotNull] string proxyUrl);

        /// <summary>
        /// With Proxy URL using X509Certificate2.
        /// </summary>
        /// <param name="proxyUrl">The proxy url.</param>
        /// <param name="clientX509Certificate2Filename">The X509Certificate2 file to use for client authentication.</param>
        /// /// <param name="clientX509Certificate2Password">The X509Certificate2 password.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithProxy([NotNull] string proxyUrl, [CanBeNull] string clientX509Certificate2Filename, [CanBeNull] string clientX509Certificate2Password);
    }
}