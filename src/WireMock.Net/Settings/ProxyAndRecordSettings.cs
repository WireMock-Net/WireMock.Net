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
        /// The clientCertificate thumbprint or subject name fragment to use. Example thumbprint : "D2DBF135A8D06ACCD0E1FAD9BFB28678DF7A9818". Example subject name: "www.google.com""
        /// </summary>
        public string X509Certificate2ThumbprintOrSubjectName { get; set; }
    }
}