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
    }
}