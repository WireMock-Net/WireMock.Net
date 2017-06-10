namespace WireMock.Settings
{
    /// <summary>
    /// RecordAndSaveSettings
    /// </summary>
    public class ProxyAndRecordSettings
    {
        /// <summary>
        /// The URL to proxy.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Save the mapping for each request/response.
        /// </summary>
        public bool SaveMapping { get; set; } = true;

        /// <summary>
        /// The clientCertificateFilename to use. Example : "C:\certificates\cert.pfx. Must be in .pfx format and include the private key"
        /// </summary>
        public string X509Certificate2Filename { get; set; }

        /// <summary>
        /// The X509Certificate2 password.
        /// </summary>
        public string X509Certificate2Password { get; set; }
    }
}