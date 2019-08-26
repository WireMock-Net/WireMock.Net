using JetBrains.Annotations;

namespace WireMock.Settings
{
    /// <summary>
    /// ProxyAndRecordSettings
    /// </summary>
    public class ProxyAndRecordSettings
    {
        /// <summary>
        /// The URL to proxy.
        /// </summary>
        [PublicAPI]
        public string Url { get; set; }

        /// <summary>
        /// Save the mapping for each request/response to the internal Mappings.
        /// </summary>
        [PublicAPI]
        public bool SaveMapping { get; set; } = true;

        /// <summary>
        /// Save the mapping for each request/response to also file. (Note that SaveMapping must also be set to true.)
        /// </summary>
        [PublicAPI]
        public bool SaveMappingToFile { get; set; } = true;

        /// <summary>
        /// The clientCertificate thumbprint or subject name fragment to use.
        /// Example thumbprint : "D2DBF135A8D06ACCD0E1FAD9BFB28678DF7A9818". Example subject name: "www.google.com""
        /// </summary>
        [PublicAPI]
        public string ClientX509Certificate2ThumbprintOrSubjectName { get; set; }

        /// <summary>
        /// Defines a list from headers which will excluded from the saved mappings.
        /// </summary>
        [PublicAPI]
        public string[] BlackListedHeaders { get; set; }

        /// <summary>
        /// Defines a list of cookies which will excluded from the saved mappings.
        /// </summary>
        [PublicAPI]
        public string[] BlackListedCookies { get; set; }
    }
}