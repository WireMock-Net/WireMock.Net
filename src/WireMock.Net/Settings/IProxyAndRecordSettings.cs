namespace WireMock.Settings
{
    /// <summary>
    /// IProxyAndRecordSettings
    /// </summary>
    public interface IProxyAndRecordSettings
    {
        /// <summary>
        /// The URL to proxy.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Save the mapping for each request/response to the internal Mappings.
        /// </summary>
        bool SaveMapping { get; set; }

        /// <summary>
        /// Save the mapping for each request/response to also file. (Note that SaveMapping must also be set to true.)
        /// </summary>
        bool SaveMappingToFile { get; set; }

        /// <summary>
        /// The clientCertificate thumbprint or subject name fragment to use.
        /// Example thumbprint : "D2DBF135A8D06ACCD0E1FAD9BFB28678DF7A9818". Example subject name: "www.google.com""
        /// </summary>
        string ClientX509Certificate2ThumbprintOrSubjectName { get; set; }

        /// <summary>
        /// Defines a list from headers which will excluded from the saved mappings.
        /// </summary>
        string[] BlackListedHeaders { get; set; }
    }
}
