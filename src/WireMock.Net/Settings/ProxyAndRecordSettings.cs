using JetBrains.Annotations;

namespace WireMock.Settings
{
    /// <summary>
    /// ProxyAndRecordSettings
    /// </summary>
    public class ProxyAndRecordSettings : IProxyAndRecordSettings
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
        /// Save the mapping for each request/response also to a file. (Note that SaveMapping must also be set to true.)
        /// </summary>
        [PublicAPI]
        public bool SaveMappingToFile { get; set; } = true;

        /// <summary>
        /// Only save request/response to the internal Mappings if the status code is included in this pattern. (Note that SaveMapping must also be set to true.)
        /// The pattern can contain a single value like "200", but also ranges like "2xx", "100,300,600" or "100-299,6xx" are supported.
        /// </summary>
        [PublicAPI]
        public string SaveMappingForStatusCodePattern { get; set; } = "*";

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

        /// <inheritdoc cref="IProxyAndRecordSettings.WebProxySettings"/>
        [PublicAPI]
        public IWebProxySettings WebProxySettings { get; set; }

        /// <inheritdoc cref="IProxyAndRecordSettings.AllowAutoRedirect"/>
        [PublicAPI]
        public bool? AllowAutoRedirect { get; set; }
    }
}