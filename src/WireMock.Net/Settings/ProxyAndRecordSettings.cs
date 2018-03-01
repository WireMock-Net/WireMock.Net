using JetBrains.Annotations;

namespace WireMock.Settings
{
    /// <summary>
    /// ProxyAndRecordSettings
    /// </summary>
    public class ProxyAndRecordSettings : IProxyAndRecordSettings
    {
        /// <inheritdoc cref="IProxyAndRecordSettings.Url"/>
        [PublicAPI]
        public string Url { get; set; }

        /// <inheritdoc cref="IProxyAndRecordSettings.SaveMapping"/>
        [PublicAPI]
        public bool SaveMapping { get; set; } = true;

        /// <inheritdoc cref="IProxyAndRecordSettings.SaveMappingToFile"/>
        [PublicAPI]
        public bool SaveMappingToFile { get; set; } = true;

        /// <inheritdoc cref="IProxyAndRecordSettings.ClientX509Certificate2ThumbprintOrSubjectName"/>
        [PublicAPI]
        public string ClientX509Certificate2ThumbprintOrSubjectName { get; set; }

        /// <inheritdoc cref="IProxyAndRecordSettings.BlackListedHeaders"/>
        [PublicAPI]
        public string[] BlackListedHeaders { get; set; }
    }
}