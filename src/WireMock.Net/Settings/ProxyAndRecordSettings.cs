using JetBrains.Annotations;

namespace WireMock.Settings
{
    /// <summary>
    /// ProxyAndRecordSettings
    /// </summary>
    public class ProxyAndRecordSettings : HttpClientSettings, IProxyAndRecordSettings
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
        public bool SaveMapping { get; set; }

        /// <summary>
        /// Save the mapping for each request/response also to a file. (Note that SaveMapping must also be set to true.)
        /// </summary>
        [PublicAPI]
        public bool SaveMappingToFile { get; set; }

        /// <summary>
        /// Only save request/response to the internal Mappings if the status code is included in this pattern. (Note that SaveMapping must also be set to true.)
        /// The pattern can contain a single value like "200", but also ranges like "2xx", "100,300,600" or "100-299,6xx" are supported.
        /// </summary>
        [PublicAPI]
        public string SaveMappingForStatusCodePattern { get; set; } = "*";

        /// <inheritdoc cref="IProxyAndRecordSettings.ExcludedHeaders"/>
        [PublicAPI]
        public string[] ExcludedHeaders { get; set; }

        /// <inheritdoc cref="IProxyAndRecordSettings.ExcludedCookies"/>
        [PublicAPI]
        public string[] ExcludedCookies { get; set; }
    }
}