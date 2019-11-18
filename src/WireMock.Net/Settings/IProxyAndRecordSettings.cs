using JetBrains.Annotations;

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
        /// Only save request/response to the internal Mappings if the status code is included in this pattern. (Note that SaveMapping must also be set to true.)
        /// The pattern can contain a single value like "200", but also ranges like "2xx", "100,300,600" or "100-299,6xx" are supported.
        /// </summary>
        [CanBeNull]
        string SaveMappingForStatusCodePattern { get; set; }

        /// <summary>
        /// Save the mapping for each request/response also to a file. (Note that SaveMapping must also be set to true.)
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

        /// <summary>
        /// Defines a list of cookies which will excluded from the saved mappings.
        /// </summary>
        string[] BlackListedCookies { get; set; }

        /// <summary>
        /// Defines the WebProxySettings.
        /// </summary>
        IWebProxySettings WebProxySettings { get; set; }

        /// <summary>
        /// Proxy requests should follow redirection (30x).
        /// </summary>
        bool? AllowAutoRedirect { get; set; }
    }
}