namespace WireMock.Settings
{
    /// <summary>
    /// If https is used, these settings can be used to configure the CertificateSettings in case a custom certificate instead the default .NET certificate should be used.
    ///
    /// X509StoreName and X509StoreLocation should be defined
    /// OR
    /// X509CertificateFilePath and X509CertificatePassword should be defined
    /// </summary>
    public interface IWireMockCertificateSettings
    {
        /// <summary>
        /// X509 StoreName
        /// </summary>
        string X509StoreName { get; set; }

        /// <summary>
        /// X509 StoreLocation
        /// </summary>
        string X509StoreLocation { get; set; }

        /// <summary>
        /// X509 Thumbprint or SubjectName (if not defined, the 'host' is used)
        /// </summary>
        string X509StoreThumbprintOrSubjectName { get; set; }

        /// <summary>
        /// X509Certificate FilePath
        /// </summary>
        string X509CertificateFilePath { get; set; }

        /// <summary>
        /// X509Certificate Password
        /// </summary>
        string X509CertificatePassword { get; set; }
    }
}