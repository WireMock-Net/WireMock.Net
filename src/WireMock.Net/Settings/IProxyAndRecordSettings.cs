using JetBrains.Annotations;

namespace WireMock.Settings
{
    /// <summary>
    /// IProxyAndRecordSettings
    /// </summary>
    public interface IProxyAndRecordSettings : IHttpClientSettings
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
        /// Save the mapping for each request/response to a .json mapping file.
        /// </summary>
        bool SaveMappingToFile { get; set; }

        /// <summary>
        /// Defines a list from headers which will be excluded from the saved mappings.
        /// </summary>
        string[] ExcludedHeaders { get; set; }

        /// <summary>
        /// Defines a list of cookies which will be excluded from the saved mappings.
        /// </summary>
        string[] ExcludedCookies { get; set; }
    }
}