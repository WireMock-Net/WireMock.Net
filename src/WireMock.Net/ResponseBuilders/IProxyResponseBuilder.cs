using JetBrains.Annotations;
using WireMock.Settings;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The ProxyResponseBuilder interface.
    /// </summary>
    public interface IProxyResponseBuilder : IStatusCodeResponseBuilder
    {
        /// <summary>
        /// WithProxy URL using Client X509Certificate2.
        /// </summary>
        /// <param name="proxyUrl">The proxy url.</param>
        /// <param name="clientX509Certificate2ThumbprintOrSubjectName">The X509Certificate2 file to use for client authentication.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithProxy([NotNull] string proxyUrl, [CanBeNull] string clientX509Certificate2ThumbprintOrSubjectName = null);

        /// <summary>
        /// WithProxy using IProxyAndRecordSettings.
        /// </summary>
        /// <param name="settings">The IProxyAndRecordSettings.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithProxy([NotNull] IProxyAndRecordSettings settings);
    }
}