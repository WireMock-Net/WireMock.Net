using JetBrains.Annotations;

namespace WireMock.Settings
{
    /// <summary>
    /// <see cref="IWireMockCertificateSettings"/>
    /// </summary>
    public class WireMockCertificateSettings : IWireMockCertificateSettings
    {
        /// <inheritdoc cref="IWireMockCertificateSettings.X509StoreName"/>
        [PublicAPI]
        public string X509StoreName { get; set; }

        /// <inheritdoc cref="IWireMockCertificateSettings.X509StoreLocation"/>
        [PublicAPI]
        public string X509StoreLocation { get; set; }

        /// <inheritdoc cref="IWireMockCertificateSettings.X509CertificateFilePath"/>
        [PublicAPI]
        public string X509CertificateFilePath { get; set; }

        /// <inheritdoc cref="IWireMockCertificateSettings.X509CertificatePassword"/>
        [PublicAPI]
        public string X509CertificatePassword { get; set; }
    }
}